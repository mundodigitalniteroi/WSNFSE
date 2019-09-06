using NFSE.Business;
using NFSE.Domain.Entities;
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
            return new Main().SolicitarEmissaoNotaFiscal(model);
        }

        [WebMethod]
        public RetornoNotaFiscal ReceberNotaFiscal(Consulta model)
        {
            return new Main().ReceberNotaFiscal(model);
        }

        [WebMethod]
        public string CancelarNotaFiscal(Cancelamento model)
        {
            return new Main().CancelarNotaFiscal(model);
        }

        //[WebMethod]
        //public string GerarNfse(int grvId, bool isDev)
        //{
        //    try
        //    {
        //        return new ControlarEnvio().GerarNota(grvId, isDev);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "NOK: " + ex.Message.ToString();
        //    }
        //}
    }
}