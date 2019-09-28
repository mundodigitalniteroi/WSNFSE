using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
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

            model.NfeId = nfe.NfeId;

            var grv = new GrvController().Selecionar(model.GrvId);

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = new DepositoController().Selecionar(grv.DepositoId).EmpresaId })) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, null, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, Empresa.Token);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro ao receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetorno(grv, nfe, model, nfse);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro ao cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetorno(GrvEntity grv, NfeEntity nfe, Consulta notaFiscalRecebida, string retorno)
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

            NfeConfiguracaoImagemEntity configuracaoImagem;

            if ((configuracaoImagem = new NfeConfiguracaoImagemController().Selecionar(grv.ClienteId, grv.DepositoId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, notaFiscalRecebida.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Retorno, "Configuração de imagem não cadastrado para o Depósito: " + grv.DepositoId);

                throw new Exception("Configuração de imagem não cadastrado para o Depósito: " + grv.DepositoId);
            }

            if (configuracaoImagem.ValueX == 0 && configuracaoImagem.ValueY == 0 && configuracaoImagem.Width == 0 && configuracaoImagem.Height == 0)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, notaFiscalRecebida.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Retorno, "Configuração de imagem cadastrado mas não configurado para o Depósito: " + grv.DepositoId);

                throw new Exception("Configuração de imagem cadastrado mas não configurado para o Depósito: " + grv.DepositoId);
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

            string directory = @"D:\Sistemas\GeradorNF\bin\NFE\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";

            Directory.CreateDirectory(directory);

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                using (var memoryStream = new MemoryStream())
                {
                    notaFiscal.UrlNotaFiscal = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                    new Tools().ObterImagemEndereco(notaFiscal.UrlNotaFiscal).Save(memoryStream, ImageFormat.Jpeg);

                    notaFiscal.ImagemNotaFiscal = memoryStream.ToArray();

                    File.WriteAllBytes(directory + nfe.NfeId + "Original.jpg", notaFiscal.ImagemNotaFiscal);
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
                    //configuracaoImagem.ValueX = 270; // Margem Esquerda
                    //configuracaoImagem.ValueY = 220; // Margem Superior
                    //configuracaoImagem.Width = 1000; // Margem Direita
                    //configuracaoImagem.Height = 1300; // Margem Inferior

                    //File.Delete(@"D:\Temp\RetornoOriginal.jpg");
                    //File.Delete(@"D:\Temp\RetornoRecortado");

                    //File.WriteAllBytes(@"D:\Temp\RetornoOriginal.jpg", notaFiscal.ImagemNotaFiscal);

                    //try
                    //{
                    //    byte[] array = CropImage(notaFiscal.ImagemNotaFiscal, new Rectangle(configuracaoImagem.ValueX, configuracaoImagem.ValueY, configuracaoImagem.Width, configuracaoImagem.Height));

                    //    File.WriteAllBytes(@"D:\Temp\RetornoRecortado.jpg", array);
                    //}
                    //catch (Exception ex)
                    //{
                    //    if (true)
                    //    {

                    //    }
                    //}
                }
                #endregion Fim do trecho para testes

                try
                {
                    notaFiscal.ImagemNotaFiscal = CropImage(notaFiscal.ImagemNotaFiscal, new Rectangle(configuracaoImagem.ValueX, configuracaoImagem.ValueY, configuracaoImagem.Width, configuracaoImagem.Height));

                    try
                    {
                        File.WriteAllBytes(directory + nfe.NfeId + ".jpg", notaFiscal.ImagemNotaFiscal);

                        if (!IsImage(notaFiscal.ImagemNotaFiscal))
                        {
                            throw new Exception("A Imagem recortada nao é uma Imagem válida");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao recortar a Imagem retornada: " + ex.Message);
                }

                retornoConsulta.ImagemNotaFiscal = notaFiscal.ImagemNotaFiscal;

                if (nfe.Status == 'A')
                {
                    nfe.Status = 'P';
                }
                else if (nfe.Status == 'S')
                {
                    nfe.Status = 'T';
                }

                new NfeImagemController().Excluir(nfe.NfeId);

                new NfeImagemController().Cadastrar(nfe.NfeId, notaFiscal.ImagemNotaFiscal);

                new NfeController().AtualizarRetornoNotaFiscal(nfe, retornoConsulta);
            }

            return retornoConsulta;
        }

        private bool IsImage(byte[] bytes)
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

        private byte[] CropImage(byte[] byteImage, Rectangle rectangle)
        {
            try
            {
                using (var memoryStream = new MemoryStream(byteImage))
                {
                    Image imageSource = Image.FromStream(memoryStream);

                    Image imageTarget = CropImage(imageSource, rectangle);

                    var converter = new ImageConverter();

                    return (byte[])converter.ConvertTo(imageTarget, typeof(byte[]));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);

            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private void AtualizarNotaFiscal(NfeEntity nfe)
        {
            nfe.Status = 'E';

            new NfeController().Atualizar(nfe);
        }
    }
}