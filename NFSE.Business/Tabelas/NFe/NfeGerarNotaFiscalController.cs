using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeGerarNotaFiscalController
    {
        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, 0, 0, usuarioId, isDev);
        }

        public List<string> GerarNovaNotaFiscal(int grvId, int identificadorNota, int faturamentoServicoTipoVeiculoId, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, identificadorNota, faturamentoServicoTipoVeiculoId, usuarioId, isDev);
        }

        private List<string> GerarNotaFiscal(int grvId, int identificadorNota, int faturamentoServicoTipoVeiculoId, int usuarioId, bool isDev)
        {
            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            DataBase.SetContextInfo(usuarioId);

            var acao = Acao.Solicitação;

            var returnList = new List<string>();

            #region NFe
            var NfeList = new List<NfeEntity>();

            // STATUS:
            //   C: Cadastro;
            //   A: Aguardando Processamento (envio da solicitação com sucesso, para a Prefeitura);
            //   P: Processado (download da Nfe e atualização da Nfe no Sistema concluídos com sucesso);
            //   R: Reprocessar (marcação manual para o envio de uma nova solicitação de Nfe para o mesmo GRV, esta opção gera um novo registro de Nfe);
            //   S: Aguardando Reprocessamento;
            //   T: Reprocessado (conclusão do reprocessamento);
            //   N: CaNcelado.
            //   E: Erro (quando a Prefeitura indicou algum problema);
            //   I: Inválido (quando ocorreu um erro Mob-Link);

            var Nfe = new NfeEntity
            {
                GrvId = grvId,

                IdentificadorNota = identificadorNota,

                FaturamentoServicoTipoVeiculoId = faturamentoServicoTipoVeiculoId
            };

            if (Nfe.IdentificadorNota > 0)
            {
                Nfe = new NfeController().Selecionar(Nfe, true);

                if (Nfe == null)
                {
                    new NfeWsErroController().CadastrarErroGenerico(Nfe.GrvId, usuarioId, Nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Nota Fiscal não encontrada");

                    returnList.Add("AVISO: Nota Fiscal não encontrada");

                    return returnList;
                }

                // Erro / Inválido / Cancelado
                if (Nfe.Status != 'E' && Nfe.Status != 'I' && Nfe.Status != 'N')
                {
                    new NfeWsErroController().CadastrarErroGenerico(Nfe.GrvId, usuarioId, Nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Nota Fiscal não está apto para reprocessamento");

                    returnList.Add("AVISO: Nota Fiscal não está apto para reprocessamento");

                    return returnList;
                }
            }
            else if ((NfeList = new NfeController().Listar(Nfe)) != null)
            {
                var status = new char[] { 'C', 'A', 'P', 'R', 'S', 'T' };

                if (NfeList.Where(w => status.Contains(w.Status)).Count() > 0)
                {
                    if (NfeList.Count == 1)
                    {
                        Nfe = NfeList.FirstOrDefault();

                        if (Nfe.Status == 'C' && Nfe.DataCadastro.Date < DateTime.Now.Date)
                        {
                            Nfe.Status = 'I';

                            new NfeController().Atualizar(Nfe);
                        }
                    }
                    else
                    {
                        new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "GRV já possui Nota Fiscal cadastrada");

                        returnList.Add("AVISO: GRV já possui Nota Fiscal cadastrada");

                        return returnList;
                    }
                }
            }
            #endregion NFe

            #region GRV
            var grv = new GrvController().Selecionar(grvId);
            #endregion Grv

            #region Cliente
            var Cliente = new ClienteController().Selecionar(grv.ClienteId);
            #endregion Cliente

            #region Depósito
            var Deposito = new DepositoController().Selecionar(grv.DepositoId);
            #endregion Depósito

            #region Regras do Faturamento
            if (new RegraFaturamentoController().Selecionar(grv.ClienteId, grv.DepositoId, new TipoRegraFaturamentoController().Selecionar("NFE").FaturamentoRegraTipoId) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "CLIDEP sem regra de NFE definido");

                returnList.Add("AVISO: CLIDEP sem regra de NFE definido");

                return returnList;
            }
            #endregion Regras do Faturamento

            #region Empresa
            var Empresa = new EmpresaEntity();
            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = Deposito.EmpresaId })) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Empresa associada não encontrada");

                returnList.Add("AVISO: Empresa associada não encontrada");

                return returnList;
            }
            #endregion Empresa

            #region Atendimento
            var Atendimento = new AtendimentoEntity();

            if ((Atendimento = new AtendimentoController().Selecionar(grvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Atendimento não encontrado");

                returnList.Add("AVISO: Atendimento não encontrado");

                return returnList;
            }
            #endregion Atendimento

            #region Faturamento
            var Faturamentos = new List<FaturamentoEntity>();

            if ((Faturamentos = new FaturamentoController().Listar(Atendimento.AtendimentoId, 'P')) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Faturamento não encontrado");

                returnList.Add("AVISO: Faturamento não encontrado");

                return returnList;
            }

            if (Faturamentos.Sum(c => c.ValorPagamento).Equals(0))
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Faturamento sem valor");

                returnList.Add("AVISO: Faturamento sem valor");

                return returnList;
            }
            #endregion Faturamento

            #region Valores somados da Composição do Faturamento
            var Composicoes = new List<FaturamentoAssociadoCnaeEntity>();

            if ((Composicoes = new FaturamentoAssociadoCnaeController().Listar(grvId, Nfe.FaturamentoServicoTipoVeiculoId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }

            Composicoes = Composicoes.Where(w => w.CnaeId > 0 && w.ListaServicoId > 0).ToList();

            if (Composicoes.Count == 0)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");

                returnList.Add("AVISO: Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");

                return returnList;
            }
            #endregion Valores somados da Composição do Faturamento

            var CapaAutorizacaoNfse = new CapaAutorizacaoNfse();

            string json;

            foreach (var composicao in Composicoes)
            {
                Nfe = new NfeEntity
                {
                    GrvId = grvId,

                    IdentificadorNota = identificadorNota,

                    FaturamentoServicoTipoVeiculoId = faturamentoServicoTipoVeiculoId
                };

                #region Preenchimento da Entidade
                try
                {
                    CapaAutorizacaoNfse = new CapaAutorizacaoNfse
                    {
                        GrvId = grvId,

                        IdentificadorNota = new DetranController().GetDetranSequence("NFE"),

                        UsuarioId = usuarioId,

                        Homologacao = isDev,

                        Autorizacao = Autorizar(grv, Deposito, Empresa, Atendimento, composicao, isDev)
                    };
                }
                catch (Exception ex)
                {
                    returnList.Add(ex.Message);

                    continue;
                }
                #endregion Preenchimento da Entidade

                if (Nfe.FaturamentoServicoTipoVeiculoId == 0)
                {
                    // Cadastro do Envio
                    Nfe = CadastrarEnvio(grvId, Empresa.Cnpj, 'E', CapaAutorizacaoNfse.IdentificadorNota, usuarioId, composicao.FaturamentoServicoTipoVeiculoId);
                }
                else
                {
                    // Cadastro do Reenvio
                    Nfe = CadastrarEnvio(grvId, Empresa.Cnpj, 'E', CapaAutorizacaoNfse.IdentificadorNota, usuarioId, composicao.FaturamentoServicoTipoVeiculoId, Nfe.NfeId);
                }


                Nfe.Cliente = Cliente.Nome;

                Nfe.Deposito = Deposito.Descricao;

                try
                {
                    json = new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse);
                }
                catch (Exception ex)
                {
                    returnList.Add(ex.Message);

                    continue;
                }

                returnList.Add(json);

                // Execução do Web Service
                try
                {
                    if (!ProcessarResultado(json, usuarioId, Nfe))
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    returnList.Add(ex.Message);

                    continue;
                }

                if (true)
                {

                }
            }

            return returnList;
        }

        private NfeEntity CadastrarEnvio(int grvId, string cnpj, char tipo, int identificadorNota, int usuarioId, int faturamentoServicoTipoVeiculoId, int nfeComplementarId = 0)
        {
            var Nfe = new NfeEntity
            {
                GrvId = grvId,

                Cnpj = cnpj,

                UsuarioCadastroId = usuarioId,

                IdentificadorNota = identificadorNota,

                FaturamentoServicoTipoVeiculoId = faturamentoServicoTipoVeiculoId
            };

            if (nfeComplementarId > 0)
            {
                Nfe.NfeComplementarId = nfeComplementarId;
            }

            Nfe.NfeId = new NfeController().Cadastrar(Nfe);

            return new NfeController().ListarPorIdentificadorNota(identificadorNota).FirstOrDefault();
        }

        private bool ProcessarResultado(string json, int usuarioId, NfeEntity nfe)
        {
            try
            {
                var results = JsonConvert.DeserializeObject<dynamic>(json);

                if (json.Contains("processando_autorizacao"))
                {
                    nfe.CodigoRetorno = int.Parse(results["ref"].ToString().Replace("{", "").Replace("}", ""));

                    new NfeMensagemController().Cadastrar(new NfeMensagemEntity
                    {
                        NfeId = nfe.NfeId,

                        Tipo = nfe.NfeComplementarId == null ? 'E' : 'R',

                        Mensagem = (string)JObject.Parse(json)["status"]
                    });

                    if (nfe.NfeComplementarId == null)
                    {
                        nfe.Status = 'A';
                    }
                    else
                    {
                        nfe.Status = 'S';
                    }                    

                    new NfeController().AguardandoProcessamento(nfe);

                    return true;
                }
                else
                {
                    var retornoErro = new NfeWsErroModel
                    {
                        GrvId = nfe.GrvId,

                        IdentificadorNota = nfe.IdentificadorNota,

                        UsuarioId = usuarioId,

                        Acao = (char)Acao.Retorno,

                        OrigemErro = (char)OrigemErro.WebService,

                        Status = results["codigo"],

                        MensagemErro = results["mensagem"]
                    };

                    retornoErro.Status = retornoErro.Status.Trim().ToUpper();

                    retornoErro.MensagemErro = retornoErro.MensagemErro.Trim();

                    retornoErro.ErroId = new NfeWsErroController().Cadastrar(retornoErro);

                    nfe.Status = 'E';

                    new NfeController().Atualizar(nfe);

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private Autorizacao Autorizar(GrvEntity grv, DepositoEntity deposito, EmpresaEntity empresa, AtendimentoEntity atendimento, FaturamentoAssociadoCnaeEntity composicao, bool isDev)
        {
            var Now = DateTime.Now;

            var Autorizacao = new Autorizacao
            {
                data_emissao = Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("HH:mm:ss"),

                natureza_operacao = "1",

                optante_simples_nacional = "false",

                prestador = Prestador(empresa, composicao.FlagEnviarInscricaoEstadual),

                tomador = Tomador(grv, deposito, atendimento)
            };

            Autorizacao.servico = Servico(grv, composicao, Autorizacao.prestador, isDev);

            return Autorizacao;
        }

        private Prestador Prestador(EmpresaEntity empresa, char flagEnviarInscricaoEstadual)
        {
            return new Prestador
            {
                cnpj = empresa.Cnpj,

                inscricao_estadual = flagEnviarInscricaoEstadual == 'S' ? empresa.InscricaoEstadual : string.Empty,

                inscricao_municipal = empresa.InscricaoMunicipal,

                codigo_municipio = new EnderecoCompletoController().Selecionar(empresa.CepId.Value).CodigoMunicipioIbge
            };
        }

        private Tomador Tomador(GrvEntity grv, DepositoEntity deposito, AtendimentoEntity atendimento)
        {
            return new Tomador
            {
                cpf = atendimento.NotaFiscalCpf.Length.Equals(11) ? atendimento.NotaFiscalCpf : string.Empty,

                cnpj = atendimento.NotaFiscalCpf.Length.Equals(14) ? atendimento.NotaFiscalCpf : string.Empty,

                razao_social = atendimento.NotaFiscalNome,

                telefone = (atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone).Length.Equals(0) ? "2199999999" : atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone,

                email = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalEmail) ? atendimento.NotaFiscalEmail : deposito.EmailNfe,

                endereco = Endereco(grv, atendimento)
            };
        }

        private Endereco Endereco(GrvEntity grv, AtendimentoEntity atendimento)
        {
            var CEP = new EnderecoCompletoController().Selecionar(atendimento.NotaFiscalCep);

            string codigo_municipio_ibge = string.Empty;

            if (CEP != null && !string.IsNullOrWhiteSpace(CEP.CodigoMunicipioIbge))
            {
                codigo_municipio_ibge = CEP.CodigoMunicipioIbge;
            }
            else
            {
                codigo_municipio_ibge = new MunicipioController().SelecionarPrimeiroCodigoIbge(atendimento.NotaFiscalUf);
            }

            return new Endereco
            {
                logradouro = atendimento.NotaFiscalEndereco,

                numero = atendimento.NotaFiscalNumero,

                complemento = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalComplemento) ? atendimento.NotaFiscalComplemento : "...",

                bairro = atendimento.NotaFiscalBairro,

                uf = atendimento.NotaFiscalUf,

                cep = atendimento.NotaFiscalCep,

                codigo_municipio = codigo_municipio_ibge
            };
        }

        private Servico Servico(GrvEntity grv, FaturamentoAssociadoCnaeEntity composicao, Prestador prestador, bool isDev)
        {
            CnaeListaServicoParametroMunicipioEntity CnaeListaServicoParametroMunicipio = new CnaeListaServicoParametroMunicipioEntity
            {
                CnaeId = composicao.CnaeId.Value,

                ListaServicoId = composicao.ListaServicoId.Value,

                CodigoMunicipioIbge = prestador.codigo_municipio
            };

            if ((CnaeListaServicoParametroMunicipio = new CnaeListaServicoParametroMunicipioController().Selecionar(CnaeListaServicoParametroMunicipio)) == null)
            {
                throw new Exception("Associação entre CNAE, Lista de Serviço e Município inexistente");
            }

            return new Servico
            {
                aliquota = string.Format("{0:N2}", CnaeListaServicoParametroMunicipio.AliquotaIss).Replace(",", "."),

                discriminacao = composicao.DescricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv,

                iss_retido = "false",

                codigo_cnae = composicao.Cnae,

                item_lista_servico = CnaeListaServicoParametroMunicipio.ListaServico,

                valor_iss = composicao.FlagEnviarValorIss == 'S' ? string.Format("{0:N2}", CnaeListaServicoParametroMunicipio.AliquotaIss / 100).Replace(",", ".") : "0",

                codigo_tributario_municipio = CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio,

                valor_servicos = isDev ? "1" : Math.Round(composicao.TotalComDesconto, 2).ToString().Replace(",", ".")
            };
        }
    }
}