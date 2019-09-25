using Newtonsoft.Json.Linq;
using NFSE.Business.Util;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using JsonUtil;
using System;
using NFSE.Domain.Entities.Global;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Tabelas.DP;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeSolicitarEmissaoNotaFiscalController
    {
        public string SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Solicitação);

            var grv = new GrvController().Selecionar(model.GrvId);

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new DepositoController().Selecionar(grv.DepositoId).EmpresaId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, null, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            string uri = new NfeConfiguracao().GetRemoteServer() + "?ref=" + model.IdentificadorNota;

            var tools = new Tools();

            var jsonUtil = new JsonUtil.JsonUtil();

            string json = tools.ObjToJSON(model.Autorizacao);

            if (string.IsNullOrWhiteSpace(model.Autorizacao.tomador.cnpj))
            {
                json = jsonUtil.RemoveElement(json, "tomador", "cnpj");
            }
            else
            {
                json = jsonUtil.RemoveElement(json, "tomador", "cpf");
            }

            string resposta;

            try
            {
                resposta = tools.PostNfse(uri, json, Empresa.Token);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Solicitação, "Ocorreu um erro ao solicitar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                new NfeRetornoSolicitacaoController().Cadastrar(nfe, model, resposta);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Solicitação, "Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            return resposta;
        }

        //public string ReceberNotaFiscal(Consultar model)
        //{
        //    return new JavaScriptSerializer().Serialize(Consultar_obj(model));
        //}
    }
}