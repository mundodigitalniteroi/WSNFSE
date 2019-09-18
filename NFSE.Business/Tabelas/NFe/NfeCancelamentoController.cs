using NFSE.Business.Util;
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

            var prestadorAcesso = new PrestadorController().ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.CnpjPrestador, Acao.Cancelamento, nfe);

            var tools = new Tools();

            string json = tools.ObjToJSON(new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    model.Justificativa
                }
            });

            try
            {
                string result = tools.CancelarNfse(prestadorAcesso.server + "/" + model.IdentificadorNota, json, prestadorAcesso.prestador_chave);

                // TODO: Cadastrar a mensagem de erro caso ocorra

                try
                {
                    nfe.Status = 'N';

                    new NfeController().Atualizar(nfe);
                }
                catch (Exception ex)
                {

                }

                return result;
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Cancelamento, "Ocorreu um erro ao cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }
    }
}
