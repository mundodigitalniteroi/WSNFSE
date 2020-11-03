using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeCancelamentoController
    {
        public string CancelarNotaFiscal(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Cancelamento);

            if (nfe == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, model.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Nota Fiscal não encontrada");

                throw new Exception("Nota Fiscal não encontrada");
            }

            var grv = new GrvController().Selecionar(model.GrvId);

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = new DepositoController().Selecionar(grv.DepositoId).EmpresaId } )) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, model.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            var tools = new Tools();

            string jsonEnvio = tools.ObjToJSON(new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    model.Justificativa
                }
            });

            string jsonRetorno;

            try
            {
                jsonRetorno = tools.CancelarNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, jsonEnvio, Empresa.Token);
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota, OrigemErro.WebService, Acao.Cancelamento, "Ocorreu um erro ao cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                var retornoConsulta = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<RetornoCancelamentoEntity>(jsonRetorno);

                if (retornoConsulta.erros != null)
                {
                    var retornoErro = new NfeWsErroModel();

                    foreach (var erro in retornoConsulta.erros)
                    {
                        retornoErro.GrvId = model.GrvId;
                        retornoErro.IdentificadorNota = model.IdentificadorNota;
                        retornoErro.UsuarioId = model.UsuarioId;
                        retornoErro.Acao = (char)Acao.Retorno;
                        retornoErro.OrigemErro = (char)OrigemErro.WebService;
                        retornoErro.Status = retornoConsulta.status.Trim().ToUpper();

                        if (erro.codigo != null)
                        {
                            retornoErro.CodigoErro = erro.codigo.Replace("  ", " ").Trim().ToUpper();
                        }

                        if (erro.mensagem != null)
                        {
                            retornoErro.MensagemErro = erro.mensagem.Replace("  ", " ").Trim();
                        }

                        if (erro.correcao != null)
                        {
                            retornoErro.CorrecaoErro = erro.correcao.Replace("  ", " ").Trim();
                        }

                        retornoErro.ErroId = new NfeWsErroController().Cadastrar(retornoErro);
                    }

                    return jsonRetorno;
                }
            }
            catch (Exception ex)
            {
                if (true)
                {

                }
            }

            

            nfe.Status = 'N';

            new NfeController().Atualizar(nfe);

            return jsonRetorno;
        }

        public string CancelarNotaFiscalAvulso(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = new NfeController().ConsultarNotaFiscal(model.IdentificadorNota);

            if (nfe == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(model.GrvId, model.UsuarioId, model.IdentificadorNota, OrigemErro.MobLink, Acao.Retorno, "Nota Fiscal não encontrada");

                throw new Exception("Nota Fiscal não encontrada");
            }

            #region Empresa
            EmpresaEntity Empresa;

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { Cnpj = model.Cnpj })) == null)
            {
                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            var tools = new Tools();

            string jsonEnvio = tools.ObjToJSON(new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    model.Justificativa
                }
            });

            string jsonRetorno;

            try
            {
                jsonRetorno = tools.CancelarNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, jsonEnvio, Empresa.Token);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            nfe.Status = 'N';

            new NfeController().Atualizar(nfe);

            return jsonRetorno;
        }
    }
}