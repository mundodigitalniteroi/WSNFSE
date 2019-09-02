using Negocio;
using Negocio.Modelo;
using System;
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
        public string Autorizar(CapaAutorizacaoNfse obj)
        {
            return new ControlarEnvio().AutorizarNfse(obj);
        }

        [WebMethod]
        public string Consultar(Consultar referencia)
        {
            return new ControlarEnvio().Consultar(referencia);
        }

        [WebMethod]
        public RetornoConsulta ConsultarRetornoClasse(Consultar referencia)
        {
            return new ControlarEnvio().Consultar_obj(referencia);
        }

        [WebMethod]
        public string ConsultarTestes(string referencia)
        {
            return new ControlarEnvio().Consultar(new Consultar()
            {
                referencia = referencia,
                homologacao = false
            });
        }

        [WebMethod]
        public RetornoConsulta ConsultarRetornoClasseTestes(string referencia)
        {
            return new ControlarEnvio().Consultar_obj(new Consultar()
            {
                referencia = referencia,
                cnpj_prestador = "25329339000248",
                homologacao = false
            });
        }

        [WebMethod]
        public string Cancelar(Cancelar referencia)
        {
            return new ControlarEnvio().Cancelar(referencia);
        }

        [WebMethod]
        public string CancelarTestes(string referencia)
        {
            return new ControlarEnvio().Cancelar(new Cancelar()
            {
                referencia = referencia,
                justificativa = "Testes de cancelamento",
                homologacao = false
            });
        }

        [WebMethod]
        public string GerarNfse(int grvId, bool isDev)
        {
            try
            {
                return new ControlarEnvio().GerarNota(grvId, isDev);
            }
            catch (Exception ex)
            {
                return "NOK: " + ex.Message.ToString();
            }
        }
    }
}