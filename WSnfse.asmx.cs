using NFSE.Business.Tabelas.NFe;
using NFSE.Business.Util;
using NFSE.Domain.Entities.NFe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;
using System.Web.Services.Description;

namespace NFSE
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSnfse : WebService
    {
        [WebMethod]
        public string SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse model)
        {
            try
            {
                return new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscal(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em SolicitarEmissaoNotaFiscal: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public string SolicitarEmissaoNotaFiscalAvulso(CapaAutorizacaoNfse model)
        {
            try
            {
                return new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscalAvulso(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em SolicitarEmissaoNotaFiscalAvulso: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            try
            {
                new Tools().GravarLog("ReceberNotaFiscal: NfeId: " + model.NfeId + "Ambiente: " + (model.Homologacao ? "DEV" : "PROD"));

                return new NfeReceberNotaFiscalController().ReceberNotaFiscal(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em ReceberNotaFiscal: " + ex.Message + "Ambiente: " + (model.Homologacao ? "DEV" : "PROD"));

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public RetornoNotaFiscalEntity ReceberNotaFiscalAvulso(Consulta model)
        {
            try
            {
                return new NfeReceberNotaFiscalController().ReceberNotaFiscalAvulso(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em ReceberNotaFiscalAvulso: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public string CancelarNotaFiscal(Cancelamento model)
        {
            try
            {
                return new NfeCancelamentoController().CancelarNotaFiscal(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em CancelarNotaFiscal: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public string CancelarNotaFiscalAvulso(Cancelamento model)
        {
            try
            {
                return new NfeCancelamentoController().CancelarNotaFiscalAvulso(model);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em CancelarNotaFiscalAvulso: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            try
            {
                return new NfeGerarNotaFiscalController().GerarNotaFiscal(grvId, usuarioId, isDev);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em GerarNotaFiscal: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public List<string> GerarNovaNotaFiscal(int grvId, string identificadorNota, int usuarioId, bool isDev)
        {
            try
            {
                return new NfeGerarNotaFiscalController().GerarNovaNotaFiscal(grvId, identificadorNota, usuarioId, isDev);
            }
            catch (Exception ex)
            {
                new Tools().GravarLog("Erro em GerarNovaNotaFiscal: " + ex.Message);

                throw new Exception(ex.Message);
            }
        }
    }
}