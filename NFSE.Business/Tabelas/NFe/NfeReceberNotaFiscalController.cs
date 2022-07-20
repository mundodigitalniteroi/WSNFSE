using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeReceberNotaFiscalController
    {
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta identificaoNotaFiscal)
        {
            DataBase.SystemEnvironment = identificaoNotaFiscal.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            NfeEntity nfe = new NfeController().ConsultarNotaFiscal(identificaoNotaFiscal.GrvId, identificaoNotaFiscal.UsuarioId, identificaoNotaFiscal.IdentificadorNota, Acao.Retorno);

            identificaoNotaFiscal.NfeId = nfe.NfeId;

            GrvEntity grv = new GrvController().Selecionar(identificaoNotaFiscal.GrvId);

            ClienteDepositoEntity ClienteDeposito = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = grv.ClienteId, DepositoId = grv.DepositoId });

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

        Outer:

            try
            {
                json = new Tools().GetNfse(new NfeConfiguracao().GetRemoteServer() + "/" + identificaoNotaFiscal.IdentificadorNota, Empresa.Token);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("por minuto"))
                {
                    goto Outer;
                }

                AtualizarStatusNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, identificaoNotaFiscal.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro ao receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + identificaoNotaFiscal.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetorno(grv, nfe, identificaoNotaFiscal, json);
            }
            catch (Exception ex)
            {
                AtualizarStatusNotaFiscal(nfe);

                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, identificaoNotaFiscal.UsuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro ao cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + identificaoNotaFiscal.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetorno(GrvEntity grv, NfeEntity nfe, Consulta identificaoNotaFiscal, string retornoJson)
        {
            RetornoNotaFiscalEntity retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retornoJson);

            if (retornoConsulta.status.Trim().Equals("processando_autorizacao", StringComparison.CurrentCultureIgnoreCase))
            {
                return retornoConsulta;
            }

            NfeWsErroModel retornoErro = new NfeWsErroModel();

            NfeWsErroController NfeWsErroController = new NfeWsErroController();

            if (retornoConsulta.erros != null)
            {
                foreach (Erros erro in retornoConsulta.erros)
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
                    retornoConsulta.ImagemNotaFiscal = BaixarImagem(grv.ClienteId, grv.DepositoId, nfe.IdentificadorNota, identificaoNotaFiscal, retornoConsulta.url);
                }

                if (identificaoNotaFiscal.BaixarImagemOriginal)
                {
                    return retornoConsulta;
                }

                nfe.Status = nfe.Status == 'A' ? 'P' : 'T';

                new NfeImagemController().Excluir(nfe.NfeId);

                new NfeImagemController().Cadastrar(nfe.NfeId, retornoConsulta.ImagemNotaFiscal);

                new NfeController().AtualizarRetornoNotaFiscal(nfe, retornoConsulta);
            }

            return retornoConsulta;
        }

        private byte[] BaixarImagem(int clienteId, int depositoId, string identificadorNota, Consulta identificaoNotaFiscal, string url)
        {
            byte[] imagemNotaFiscal;

            NfeConfiguracaoImagemEntity ConfiguracaoImagem;

            NfeConfiguracaoImagemController NfeConfiguracaoImagemController = new NfeConfiguracaoImagemController();

            if ((ConfiguracaoImagem = NfeConfiguracaoImagemController.Selecionar(new NfeConfiguracaoImagemEntity { ClienteId = clienteId, DepositoId = depositoId })) == null)
            {
                ConfiguracaoImagem = new NfeConfiguracaoImagemEntity
                {
                    ClienteDepositoId = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = clienteId, DepositoId = depositoId }).ClienteDepositoId,

                    ValueX = 10,

                    ValueY = 10,

                    Height = 500,

                    Width = 500
                };

                ConfiguracaoImagem.ConfiguracaoImagemId = NfeConfiguracaoImagemController.Cadastrar(ConfiguracaoImagem);
            }

            string directory;

            string drive = new Tools().DriveToSave();

            directory = $@"{drive}Sistemas\GeradorNF\NFE\" + DataBase.SystemEnvironment.ToString() + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            List<NfeRegraEntity> regrasNfe = new NfeRegraController().Listar(new NfeRegraEntity()
            {
                ClienteId = clienteId,
                DepositoId = depositoId
            });

            if (regrasNfe?.Count(w => w.RegraCodigo.Equals("NFPDF") && w.Ativo.Equals(1)) > 0 || url.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                using (WebClient webClient = new WebClient())
                {
                    string str1 = directory + identificadorNota + "Original.pdf";
                    string str2 = directory + identificadorNota + ".jpg";

                    webClient.Headers.Add("user-agent", "Mob-Link");

                    webClient.DownloadFile(url, str1);

                    PdfToJpg.Process(str1, str2);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        Image.FromFile(str2).Save(memoryStream, ImageFormat.Jpeg);

                        imagemNotaFiscal = memoryStream.ToArray();
                    }
                }
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Tools.ObterImagemEndereco(url).Save(memoryStream, ImageFormat.Jpeg);

                    imagemNotaFiscal = memoryStream.ToArray();
                }

                try
                {
                    if (!IsImage(imagemNotaFiscal))
                    {
                        throw new Exception("A Imagem retornada nao é uma Imagem válida");
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Ocorreu um erro ao analisar a Imagem retornada");
                }

                #region Início do trecho para testes
                //if (new[] { "BETODELL", "BETOLENOVO", "SUPREMELEADER" }.Contains(SystemInformation.ComputerName))
                //{
                //    configuracaoImagem.ValueX = 0; // Margem Esquerda
                //    configuracaoImagem.ValueY = 0; // Margem Superior
                //    configuracaoImagem.Width = 1035; // Margem Direita
                //    configuracaoImagem.Height = 1315; // Margem Inferior
                //}
                #endregion Fim do trecho para testes

                try
                {
                    if (!identificaoNotaFiscal.BaixarImagemOriginal)
                    {
                        imagemNotaFiscal = CropImage(imagemNotaFiscal, new Rectangle(ConfiguracaoImagem.ValueX, ConfiguracaoImagem.ValueY, ConfiguracaoImagem.Width, ConfiguracaoImagem.Height));

                        // File.WriteAllBytes(directory + identificadorNota + "Recortado.jpg", ImagemNotaFiscal);

                        if (!IsImage(imagemNotaFiscal))
                        {
                            throw new Exception("A Imagem recortada nao é uma Imagem válida");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ocorreu um erro ao recortar a Imagem retornada: " + ex.Message);
                }
            }

            return imagemNotaFiscal;
        }

        private bool IsImage(byte[] bytes)
        {
            byte[] bmp = Encoding.ASCII.GetBytes("BM"); // BMP
            byte[] gif = Encoding.ASCII.GetBytes("GIF"); // GIF
            byte[] jpeg = new byte[] { 255, 216, 255, 224 }; // JPEG
            byte[] jpeg2 = new byte[] { 255, 216, 255, 225 }; // JPEG Canon
            byte[] png = new byte[] { 137, 80, 78, 71 }; // PNG
            byte[] tiff = new byte[] { 73, 73, 42 }; // TIFF
            byte[] tiff2 = new byte[] { 77, 77, 42 }; // TIFF

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
                using (MemoryStream memoryStream = new MemoryStream(byteImage))
                {
                    Image imageSource = Image.FromStream(memoryStream);

                    Image imageTarget = CropImage(imageSource, rectangle);

                    ImageConverter converter = new ImageConverter();

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

        private void AtualizarStatusNotaFiscal(NfeEntity nfe)
        {
            if (nfe.Status == 'P' || nfe.Status == 'T')
            {
                return;
            }

            nfe.Status = 'E';

            new NfeController().Atualizar(nfe);
        }

        public RetornoNotaFiscalEntity ReceberNotaFiscalAvulso(Consulta model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            NfeEntity nfe = new NfeController().ConsultarNotaFiscal(model.IdentificadorNota);

            model.NfeId = nfe.NfeId;

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { Cnpj = model.Cnpj })) == null)
            {
                throw new Exception("Empresa associada não encontrada");
            }

            if (Empresa.Token == null)
            {
                throw new Exception("O Token não foi configurado");
            }
            #endregion Empresa

            string json;

        Outer:

            try
            {
                json = new Tools().GetNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, Empresa.Token);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("por minuto"))
                {
                    goto Outer;
                }

                AtualizarStatusNotaFiscal(nfe);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetornoAvulso(nfe, model, json);
            }
            catch (Exception ex)
            {
                AtualizarStatusNotaFiscal(nfe);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetornoAvulso(NfeEntity nfe, Consulta identificaoNotaFiscal, string retornoJson)
        {
            RetornoNotaFiscalEntity retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retornoJson);

            if (retornoConsulta.status.Trim().Equals("processando_autorizacao", StringComparison.CurrentCultureIgnoreCase))
            {
                return retornoConsulta;
            }

            if (retornoConsulta.erros != null)
            {
                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                return retornoConsulta;
            }

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
                {
                    retornoConsulta.Html = BaixarImagemAvulsa(nfe.IdentificadorNota, retornoConsulta.url);
                }

                if (identificaoNotaFiscal.BaixarImagemOriginal)
                {
                    return retornoConsulta;
                }

                nfe.Status = nfe.Status == 'A' ? 'P' : 'T';

                new NfeController().AtualizarRetornoNotaFiscal(nfe, retornoConsulta);
            }

            return retornoConsulta;
        }

        private string BaixarImagemAvulsa(string identificadorNota, string url)
        {
            string directory;

            string drive = new Tools().DriveToSave();

            directory = $@"{drive}Sistemas\GeradorNF\NFE\" + DataBase.SystemEnvironment.ToString() + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string str1 = directory + identificadorNota + "Original.html";

            if (File.Exists(str1))
            {
                File.Delete(str1);
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", "Mob-Link");

                webClient.DownloadFile(url, str1);
            };

            return str1;
        }
    }
}