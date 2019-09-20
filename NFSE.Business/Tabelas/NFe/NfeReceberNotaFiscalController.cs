using NFSE.Business.Tabelas.DP;
using NFSE.Business.Util;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeReceberNotaFiscalController
    {
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Retorno);

            var grv = new GrvController().Selecionar(model.GrvId);

            NfeConfiguracaoImagemEntity configuracaoImagem;

            if ((configuracaoImagem = new NfeConfiguracaoImagemController().Selecionar(grv.ClienteId, grv.DepositoId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Configuração de imagem não cadastrado para o Depósito: " + grv.DepositoId);

                throw new Exception("Configuração de imagem não cadastrado para o Depósito: " + grv.DepositoId);
            }

            if (configuracaoImagem.ValueX == 0 && configuracaoImagem.ValueY == 0 && configuracaoImagem.Width == 0 && configuracaoImagem.Height == 0)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Configuração de imagem cadastrado mas não configurado para o Depósito: " + grv.DepositoId);

                throw new Exception("Configuração de imagem cadastrado mas não configurado para o Depósito: " + grv.DepositoId);
            }

            model.NfeId = nfe.NfeId;

            var prestadorAcesso = new PrestadorController().ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.CnpjPrestador, Acao.Retorno, nfe);

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "/" + model.IdentificadorNota, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro ao receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetorno(nfe, model, nfse, configuracaoImagem);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro ao cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetorno(NfeEntity nfe, Consulta notaFiscalRecebida, string retorno, NfeConfiguracaoImagemEntity configuracaoImagem)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retorno);

            if (retornoConsulta.status.Trim().ToLower().Equals("processando_autorizacao"))
            {
                return retornoConsulta;
            }

            if (retornoConsulta.erros != null)
            {
                var retornoErro = new NfeWsErroModel();

                foreach (var erro in retornoConsulta.erros)
                {
                    retornoErro.GrvId = notaFiscalRecebida.GrvId;
                    retornoErro.IdentificadorNota = notaFiscalRecebida.IdentificadorNota;
                    retornoErro.UsuarioId = notaFiscalRecebida.UsuarioId;
                    retornoErro.Acao = (char)Acao.Retorno;
                    retornoErro.OrigemErro = (char)OrigemErro.WebService;
                    retornoErro.Status = retornoConsulta.status.Trim().ToUpper();

                    if (erro.codigo != null)
                    {
                        retornoErro.CodigoErro = erro.codigo.Replace("  ", " ").Trim().ToUpper();
                    }

                    if (erro.mensagem != null)
                    {
                        retornoErro.MensagemErro = erro.mensagem.Replace("  ", " ").Trim();
                    }

                    if (erro.correcao != null)
                    {
                        retornoErro.CorrecaoErro = erro.correcao.Replace("  ", " ").Trim();
                    }

                    retornoErro.ErroId = new NfeWsErroController().Cadastrar(retornoErro);
                }

                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                return retornoConsulta;
            }

            var notaFiscal = new NfeRetornoModel
            {
                NfeId = notaFiscalRecebida.NfeId,
                UsuarioId = notaFiscalRecebida.UsuarioId,
                Status = retornoConsulta.status.ToUpper(),
                NumeroNotaFiscal = retornoConsulta.numero_rps,
                CodigoVerificacao = retornoConsulta.codigo_verificacao.Trim(),
                UrlNotaFiscal = retornoConsulta.url,
                CaminhoXmlNotaFiscal = retornoConsulta.caminho_xml_nota_fiscal,
                DataEmissao = retornoConsulta.data_emissao
            };

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                using (var memoryStream = new MemoryStream())
                {
                    notaFiscal.UrlNotaFiscal = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                    new Tools().ObterImagemEndereco(notaFiscal.UrlNotaFiscal).Save(memoryStream, ImageFormat.Jpeg);

                    notaFiscal.ImagemNotaFiscal = memoryStream.ToArray();
                }

                try
                {
                    if (!IsImage(notaFiscal.ImagemNotaFiscal))
                    {
                        throw new Exception("A Imagem retornada nao é uma Imagem válida");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Ocorreu um erro ao analisar a Imagem retornada");
                }

                #region Início do trecho para testes
                if (new[] { "BETODELL", "BETOLENOVO", "SUPREMELEADER" }.Contains(SystemInformation.ComputerName))
                {
                    //configuracaoImagem.ValueX = 270; // Vertical
                    //configuracaoImagem.ValueY = 220; // Top
                    //configuracaoImagem.Width = 1270; // Largura
                    //configuracaoImagem.Height = 1525; // Comprimento

                    //File.Delete(@"D:\Temp\RetornoOriginal.jpg");
                    //File.Delete(@"D:\Temp\RetornoRecortado");

                    //File.WriteAllBytes(@"D:\Temp\RetornoOriginal.jpg", notaFiscal.ImagemNotaFiscal);

                    //byte[] array = CropImage(notaFiscal.ImagemNotaFiscal, new Rectangle(configuracaoImagem.ValueX, configuracaoImagem.ValueY, configuracaoImagem.Width, configuracaoImagem.Height));

                    //File.WriteAllBytes(@"D:\Temp\RetornoRecortado" /* + DateTime.Now.ToString("yyyyMMddHHmmss") */ + ".jpg", array);
                }
                #endregion Fim do trecho para testes

                try
                {
                    notaFiscal.ImagemNotaFiscal = CropImage(notaFiscal.ImagemNotaFiscal, new Rectangle(configuracaoImagem.ValueX, configuracaoImagem.ValueY, configuracaoImagem.Width, configuracaoImagem.Height));
                }
                catch (Exception)
                {
                    throw new Exception("Ocorreu um erro ao analisar a Imagem retornada");
                }

                retornoConsulta.ImagemNotaFiscal = notaFiscal.ImagemNotaFiscal;

                if (nfe.Status == 'A')
                {
                    nfe.Status = 'P';
                }
                else if (nfe.Status == 'R')
                {
                    nfe.Status = 'R';
                }

                new NfeImagemController().Excluir(nfe.NfeId);

                new NfeImagemController().Cadastrar(nfe.NfeId, notaFiscal.ImagemNotaFiscal);

                new NfeController().AtualizarRetornoNotaFiscal(nfe.NfeId, retornoConsulta);
            }

            return retornoConsulta;
        }

        public bool IsImage(byte[] bytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM"); // BMP
            var gif = Encoding.ASCII.GetBytes("GIF"); // GIF
            var jpeg = new byte[] { 255, 216, 255, 224 }; // JPEG
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // JPEG Canon
            var png = new byte[] { 137, 80, 78, 71 }; // PNG
            var tiff = new byte[] { 73, 73, 42 }; // TIFF
            var tiff2 = new byte[] { 77, 77, 42 }; // TIFF

            if (bmp.SequenceEqual(bytes.Take(bmp.Length)) ||
                gif.SequenceEqual(bytes.Take(gif.Length)) ||
                png.SequenceEqual(bytes.Take(png.Length)) ||
                tiff.SequenceEqual(bytes.Take(tiff.Length)) ||
                tiff2.SequenceEqual(bytes.Take(tiff2.Length)) ||
                jpeg.SequenceEqual(bytes.Take(jpeg.Length)) ||
                jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
            {
                return true;
            }

            return false;
        }


        public byte[] CropImage(byte[] byteImage, Rectangle rectangle)
        {
            Image image;

            try
            {
                using (var memoryStream = new MemoryStream(byteImage))
                {
                    image = Image.FromStream(memoryStream);
                }

                var croppedImage = new Bitmap(rectangle.Width, rectangle.Height);

                using (var graphics = Graphics.FromImage(croppedImage))
                {
                    graphics.DrawImage(image, rectangle, rectangle, GraphicsUnit.Pixel);
                }

                var converter = new ImageConverter();

                return (byte[])converter.ConvertTo(croppedImage, typeof(byte[]));
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void AtualizarNotaFiscal(NfeEntity nfe)
        {
            nfe.Status = 'E';

            new NfeController().Atualizar(nfe);
        }
    }
}