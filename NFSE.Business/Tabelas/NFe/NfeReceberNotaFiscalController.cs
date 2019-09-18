using Newtonsoft.Json.Linq;
using NFSE.Business.Util;
using NFSE.Domain.Entities;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Script.Serialization;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeReceberNotaFiscalController
    {
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Retorno);

            model.NfeId = nfe.NfeId;

            var prestadorAcesso = new PrestadorController().ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.CnpjPrestador, Acao.Retorno, nfe);

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "/" + model.IdentificadorNota, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro ao receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return CadastrarNotaFiscalRecebida(model, nfse);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro ao cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity CadastrarNotaFiscalRecebida(Consulta notaFiscalRecebida, string retorno)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retorno);

            if (retornoConsulta.Status.Trim().ToUpper().Contains("PROCESSANDO_AUTORIZACAO"))
            {
                return retornoConsulta;
            }

            if (retornoConsulta.url == null)
            {
                retorno = retorno.Replace("\"codigo\"", "\"CodigoErro\"");
                retorno = retorno.Replace("\"mensagem\"", "\"MensagemErro\"");
                retorno = retorno.Replace("\"correcao\"", "\"CorrecaoErro\"");
                retorno = retorno.Replace("[", "");
                retorno = retorno.Replace("]", "");

                var retornoErro = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<NfeWsErroModel>(retorno);

                JObject jsonErro = JObject.Parse(retorno);

                if ((string)jsonErro.SelectToken("erros.CodigoErro") != null)
                {
                    retornoErro.CodigoErro = ((string)jsonErro.SelectToken("erros.CodigoErro")).Trim().ToUpper();
                }

                if ((string)jsonErro.SelectToken("erros.MensagemErro") != null)
                {
                    retornoErro.MensagemErro = ((string)jsonErro.SelectToken("erros.MensagemErro")).Trim();
                }

                if ((string)jsonErro.SelectToken("erros.CorrecaoErro") != null)
                {
                    retornoErro.CorrecaoErro = ((string)jsonErro.SelectToken("erros.CorrecaoErro")).Trim();
                }

                retornoErro.IdentificadorNota = notaFiscalRecebida.IdentificadorNota;
                retornoErro.UsuarioId = notaFiscalRecebida.UsuarioId;
                retornoErro.Acao = (char)Acao.Retorno;
                retornoErro.OrigemErro = (char)OrigemErro.WebService;
                retornoErro.Status = retornoErro.Status.Trim().ToUpper();

                retornoErro.ErroId = new NfeWsErroController().Cadastrar(retornoErro);

                retornoErro = new NfeWsErroController().Selecionar(new NfeWsErroModel { ErroId = retornoErro.ErroId });

                retornoConsulta.ErroId = retornoErro.ErroId;
                retornoConsulta.IdentificadorNota = retornoErro.IdentificadorNota;
                retornoConsulta.UsuarioId = retornoErro.UsuarioId;
                retornoConsulta.Acao = retornoErro.Acao;
                retornoConsulta.OrigemErro = retornoErro.OrigemErro;
                retornoConsulta.Status = retornoErro.Status;
                retornoConsulta.CodigoErro = retornoErro.CodigoErro;
                retornoConsulta.MensagemErro = retornoErro.MensagemErro;
                retornoConsulta.CorrecaoErro = retornoErro.CorrecaoErro;
                retornoConsulta.DataHoraCadastro = retornoErro.DataHoraCadastro;

                return retornoConsulta;
            }

            var notaFiscal = new NfeRetornoModel
            {
                NfeId = notaFiscalRecebida.NfeId,
                UsuarioId = notaFiscalRecebida.UsuarioId,
                Status = retornoConsulta.Status.ToUpper(),
                NumeroNotaFiscal = retornoConsulta.numero,
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

                retornoConsulta.ImagemNotaFiscal = notaFiscal.ImagemNotaFiscal;
            }

            retornoConsulta.NotaFiscalId = new NfeRetornoController().Cadastrar(notaFiscal);

            return retornoConsulta;
        }
    }
}