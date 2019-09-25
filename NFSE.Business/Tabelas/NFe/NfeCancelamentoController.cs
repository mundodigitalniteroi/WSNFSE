using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeCancelamentoController
    {
        public string CancelarNotaFiscal(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Cancelamento);

            var grv = new GrvController().Selecionar(model.GrvId);

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new DepositoController().Selecionar(grv.DepositoId).EmpresaId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, model.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            var tools = new Tools();

            string json = tools.ObjToJSON(new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    model.Justificativa
                }
            });

            string result;

            try
            {
                result = tools.CancelarNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, json, Empresa.Token);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Cancelamento, "Ocorreu um erro ao cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            nfe.Status = 'N';

            new NfeController().Atualizar(nfe);

            return result;
        }
    }
}