using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Web.Script.Serialization;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeSolicitarEmissaoNotaFiscalController
    {
        public string SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Solicitação);

            var grv = new GrvController().Selecionar(model.GrvId);

            var ClienteDeposito = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = grv.ClienteId, DepositoId = grv.DepositoId });

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = ClienteDeposito.EmpresaId })) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, null, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            string resposta;

            string json;

            try
            {
                json = CreateJson(model);
            }
            catch (Exception ex)
            {
                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Solicitação, "Ocorreu um erro ao criar o JSON da Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao criar o JSON da Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                resposta = new Tools().PostNfse
                (
                    uri: new NfeConfiguracao().GetRemoteServer() + "?ref=" + model.IdentificadorNota,
                    json: json,
                    token: Empresa.Token
                );
            }
            catch (Exception ex)
            {
                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Solicitação, "Ocorreu um erro ao solicitar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                new NfeRetornoSolicitacaoController().Cadastrar(nfe, model, resposta, json);
            }
            catch (Exception ex)
            {
                nfe.Status = 'E';

                new NfeController().Atualizar(nfe);

                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, Acao.Solicitação, "Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            return resposta;
        }

        public string SolicitarEmissaoNotaFiscalAvulso(CapaAutorizacaoNfse model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            NfePrestadorAvulsoEntity PrestadorAvulso;

            if ((PrestadorAvulso = NfePrestadorAvulsoPersistence.Selecionar(new NfePrestadorAvulsoEntity { Cnpj = model.Autorizacao.prestador.cnpj })) == null)
            {
                throw new Exception("Prestador não encontrado");
            }
            else if (PrestadorAvulso.Token == null)
            {
                throw new Exception("Prestador não possui Token configurado");
            }

            try
            {
                var json = new Tools().PostNfse
                (
                    uri: new NfeConfiguracao().GetRemoteServer() + "?ref=" + model.IdentificadorNota,
                    json: CreateJson(model),
                    token: PrestadorAvulso.Token
                );

                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private string CreateJson(CapaAutorizacaoNfse model)
        {
            var tools = new Tools();

            string json = tools.ObjToJSON(model.Autorizacao);

            var jsonUtil = new JsonUtil.JsonUtil();

            if (string.IsNullOrWhiteSpace(model.Autorizacao.tomador.cnpj))
            {
                json = jsonUtil.RemoveElement(json, "tomador", "cnpj");
            }
            else
            {
                json = jsonUtil.RemoveElement(json, "tomador", "cpf");
            }

            return json;
        }
    }
}