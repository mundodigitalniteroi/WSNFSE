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
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta identificaoNotaFiscal)
        {
            DataBase.SystemEnvironment = identificaoNotaFiscal.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(identificaoNotaFiscal.GrvId, identificaoNotaFiscal.UsuarioId, identificaoNotaFiscal.IdentificadorNota, Acao.Retorno);

            identificaoNotaFiscal.NfeId = nfe.NfeId;

            var grv = new GrvController().Selecionar(identificaoNotaFiscal.GrvId);

            #region Cliente Depósito
            var ClienteDeposito = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = grv.ClienteId, DepositoId = grv.DepositoId });
            #endregion Cliente Depósito

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = ClienteDeposito.EmpresaId })) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(identificaoNotaFiscal.GrvId, identificaoNotaFiscal.UsuarioId, null, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }

            if (Empresa.Token == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(identificaoNotaFiscal.GrvId, identificaoNotaFiscal.UsuarioId, null, OrigemErro.MobLink, Acao.Retorno, "O Token não foi configurado");

                throw new Exception("O Token não foi configurado");
            }

            #endregion Empresa

            string json;

            try
            {
                json = new Tools().GetNfse(new NfeConfiguracao().GetRemoteServer() + "/" + identificaoNotaFiscal.IdentificadorNota, Empresa.Token);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, identificaoNotaFiscal.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro ao receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + identificaoNotaFiscal.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetorno(grv, nfe, identificaoNotaFiscal, json);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, identificaoNotaFiscal.UsuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro ao cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + identificaoNotaFiscal.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetorno(GrvEntity grv, NfeEntity nfe, Consulta identificaoNotaFiscal, string retornoJson)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retornoJson);

            if (retornoConsulta.status.Trim().ToLower().Equals("processando_autorizacao"))
            {
                return retornoConsulta;
            }

            var retornoErro = new NfeWsErroModel();

            var NfeWsErroController = new NfeWsErroController();

            if (retornoConsulta.erros != null)
            {
                foreach (var erro in retornoConsulta.erros)
                {
                    retornoErro.GrvId = identificaoNotaFiscal.GrvId;
                    retornoErro.IdentificadorNota = identificaoNotaFiscal.IdentificadorNota;
                    retornoErro.UsuarioId = identificaoNotaFiscal.UsuarioId;
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

                    retornoErro.ErroId = NfeWsErroController.Cadastrar(retornoErro);
                }

                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                return retornoConsulta;
            }
            else
            {
                retornoErro.GrvId = identificaoNotaFiscal.GrvId;
                retornoErro.IdentificadorNota = identificaoNotaFiscal.IdentificadorNota;
                retornoErro.UsuarioId = identificaoNotaFiscal.UsuarioId;
                retornoErro.Acao = (char)Acao.Retorno;
                retornoErro.OrigemErro = (char)OrigemErro.WebService;
                retornoErro.Status = retornoConsulta.status.Trim().ToUpper();

                NfeWsErroController.Cadastrar(retornoErro);
            }

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
                {
                    retornoConsulta.ImagemNotaFiscal = BaixarImagem(grv.ClienteId, grv.DepositoId, nfe.GrvId, nfe.IdentificadorNota, identificaoNotaFiscal, retornoConsulta.url);
                }

                if (identificaoNotaFiscal.BaixarImagemOriginal)
                {
                    return retornoConsulta;
                }

                nfe.Status = nfe.Status == 'A' || nfe.Status == 'E' || nfe.Status == 'I' ? 'P' : 'T';

                new NfeImagemController().Excluir(nfe.NfeId);

                new NfeImagemController().Cadastrar(nfe.NfeId, retornoConsulta.ImagemNotaFiscal);

                new NfeController().AtualizarRetornoNotaFiscal(nfe, retornoConsulta);
            }

            return retornoConsulta;
        }

        private byte[] BaixarImagem(int clienteId, int depositoId, int grvId, int identificadorNota, Consulta identificaoNotaFiscal, string url)
        {
            byte[] ImagemNotaFiscal;

            NfeConfiguracaoImagemEntity ConfiguracaoImagem;

            NfeConfiguracaoImagemController NfeConfiguracaoImagemController = new NfeConfiguracaoImagemController();

            if ((ConfiguracaoImagem = NfeConfiguracaoImagemController.Selecionar(new NfeConfiguracaoImagemEntity { ClienteId = clienteId, DepositoId = depositoId } )) == null)
            {
                ConfiguracaoImagem.ClienteDepositoId = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = clienteId, DepositoId = depositoId }).ClienteDepositoId;

                ConfiguracaoImagem.ValueX = 100;

                ConfiguracaoImagem.ValueX = 100;

                ConfiguracaoImagem.ValueX = 100;

                ConfiguracaoImagem.ValueX = 100;

                ConfiguracaoImagem.ConfiguracaoImagemId = NfeConfiguracaoImagemController.Cadastrar(ConfiguracaoImagem);
            }

            string directory = @"D:\Sistemas\GeradorNF\NFE\" + DataBase.SystemEnvironment.ToString() + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";

            Directory.CreateDirectory(directory);

            using (var memoryStream = new MemoryStream())
            {
                new Tools().ObterImagemEndereco(url).Save(memoryStream, ImageFormat.Jpeg);

                ImagemNotaFiscal = memoryStream.ToArray();

                File.WriteAllBytes(directory + identificadorNota + "Original.jpg", ImagemNotaFiscal);
            }

            try
            {
                if (!IsImage(ImagemNotaFiscal))
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
                //configuracaoImagem.ValueX = 0; // Margem Esquerda
                //configuracaoImagem.ValueY = 0; // Margem Superior
                //configuracaoImagem.Width = 1035; // Margem Direita
                //configuracaoImagem.Height = 1315; // Margem Inferior
            }
            #endregion Fim do trecho para testes

            try
            {
                if (!identificaoNotaFiscal.BaixarImagemOriginal)
                {
                    ImagemNotaFiscal = CropImage(ImagemNotaFiscal, new Rectangle(ConfiguracaoImagem.ValueX, ConfiguracaoImagem.ValueY, ConfiguracaoImagem.Width, ConfiguracaoImagem.Height));

                    File.WriteAllBytes(directory + identificadorNota + "Recortado.jpg", ImagemNotaFiscal);

                    if (!IsImage(ImagemNotaFiscal))
                    {
                        throw new Exception("A Imagem recortada nao é uma Imagem válida");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao recortar a Imagem retornada: " + ex.Message);
            }

            return ImagemNotaFiscal;
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