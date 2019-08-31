using Negocio;
using Negocio.Modelo;
using System;
using System.Web.Services;

namespace NFSE
{
    /// <summary>
    /// Summary description for WSnfse
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
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
        public string Consultar_testes(string referencia)
        {
            return new ControlarEnvio().Consultar(new Consultar()
            {
                referencia = referencia,
                homologacao = false
            });
        }

        [WebMethod]
        public RetornoConsulta ConsultarRetornoClassetestes(string referencia)
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
        public string Cancelar_testes(string referencia)
        {
            return new ControlarEnvio().Cancelar(new Cancelar()
            {
                referencia = referencia,
                justificativa = "Testes de cancelamento",
                homologacao = false
            });
        }

        [WebMethod]
        public string GerarNfse(int id_grv, bool _isdev)
        {
            ControlarEnvio controlarEnvio = new ControlarEnvio();
            try
            {
                return controlarEnvio.GerarNota(id_grv, _isdev);
            }
            catch (Exception ex)
            {
                return "NOK: " + ex.Message.ToString();
            }
        }
    }
}
