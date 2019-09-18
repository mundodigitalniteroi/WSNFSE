using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.NFe;
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
            return new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscal(model);
        }

        [WebMethod]
        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            return new NfeReceberNotaFiscalController().ReceberNotaFiscal(model);
        }

        [WebMethod]
        public string CancelarNotaFiscal(Cancelamento model)
        {
            return new NfeCancelamentoController().CancelarNotaFiscal(model);
        }

        [WebMethod]
        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            return new NfeGerarNotaFiscalController().GerarNotaFiscal(grvId, usuarioId, isDev);
        }
    }
}