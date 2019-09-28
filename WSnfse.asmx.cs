using NFSE.Business.Tabelas.NFe;
using NFSE.Business.Util;
using NFSE.Domain.Entities.NFe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;

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
                new EmailController().Enviar(ex.Message, model.Homologacao);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public string SimularEmissaoNotaFiscal(CapaAutorizacaoNfse model)
        {
            try
            {
                return new NfeSolicitarEmissaoNotaFiscalController().SimularEmissaoNotaFiscal(model);
            }
            catch (Exception ex)
            {
                new EmailController().Enviar(ex.Message, model.Homologacao);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            try
            {
                return new NfeReceberNotaFiscalController().ReceberNotaFiscal(model);
            }
            catch (Exception ex)
            {
                new EmailController().Enviar(ex.Message, model.Homologacao);

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
                new EmailController().Enviar(ex.Message, model.Homologacao);

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
                new EmailController().Enviar(ex.Message, isDev);

                throw new Exception(ex.Message);
            }
        }

        [WebMethod]
        public List<string> GerarNovaNotaFiscal(int grvId, int faturamentoServicoTipoVeiculoId, int usuarioId, bool isDev)
        {
            try
            {
                return new NfeGerarNotaFiscalController().GerarNovaNotaFiscal(grvId, faturamentoServicoTipoVeiculoId, usuarioId, isDev);
            }
            catch (Exception ex)
            {
                new EmailController().Enviar(ex.Message, isDev);

                throw new Exception(ex.Message);
            }
        }
    }
}