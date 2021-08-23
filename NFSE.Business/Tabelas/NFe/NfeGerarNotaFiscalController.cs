using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeGerarNotaFiscalController
    {
        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, "", usuarioId, isDev);
        }

        public List<string> GerarNovaNotaFiscal(int grvId, string identificadorNota, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, identificadorNota, usuarioId, isDev);
        }

        private List<string> GerarNotaFiscal(int grvId, string identificadorNota, int usuarioId, bool isDev)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

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
            //   M: Cadastro Manual

            var Nfe = new NfeEntity
            {
                GrvId = grvId,

                IdentificadorNota = identificadorNota
            };

            if (!string.IsNullOrWhiteSpace(Nfe.IdentificadorNota))
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
                var status = new char[] { 'C', 'A', 'P', 'R', 'S', 'T', 'M' };

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
                        else
                        {
                            new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "GRV já possui Nota Fiscal cadastrada");

                            returnList.Add("AVISO: GRV já possui Nota Fiscal cadastrada");

                            return returnList;
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

            // GRV
            var grv = new GrvController().Selecionar(grvId);

            // Cliente
            var Cliente = new ClienteController().Selecionar(grv.ClienteId);

            // Depósito
            var Deposito = new DepositoController().Selecionar(grv.DepositoId);

            // Cliente Depósito
            var ClienteDeposito = new ClienteDepositoController().Selecionar(new ClienteDepositoEntity { ClienteId = grv.ClienteId, DepositoId = grv.DepositoId });

            #region Regras da Nfe
            var NfeRegras = new NfeRegraController().Listar(new NfeRegraEntity
            {
                ClienteDepositoId = ClienteDeposito.ClienteDepositoId,

                Ativo = 1,

                RegraAtivo = 1
            });
            #endregion Regras da Nfe

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

            if ((Empresa = new EmpresaController().Selecionar(new EmpresaEntity { EmpresaId = ClienteDeposito.EmpresaId })) == null)
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

                returnList.Add("AVISO: Faturamento não encontrado ou sem valor para pagamento");

                return returnList;
            }
            #endregion Faturamento

            #region Valores somados da Composição do Faturamento
            var ComposicoesAgrupadas = new List<NfeViewFaturamentoComposicaoAgrupadoEntity>();

            if ((ComposicoesAgrupadas = new NfeViewFaturamentoComposicaoAgrupadoController().Listar(grvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrada");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }
            #endregion Valores somados da Composição do Faturamento

            #region Valores somados da Composição do Faturamento
            var ComposicoesAgrupadasDescricao = new List<NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity>();

            if ((ComposicoesAgrupadasDescricao = new NfeViewFaturamentoComposicaoAgrupadoDescricaoController().Listar(grvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrada");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }
            #endregion Valores somados da Composição do Faturamento

            #region Composições
            var Composicoes = new List<NfeViewFaturamentoComposicaoEntity>();

            if ((Composicoes = new NfeViewFaturamentoComposicaoController().Listar(grvId, Nfe.NfeId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento da Nota Fiscal não encontrada");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }
            #endregion Composições

            if (Nfe.NfeId > 0)
            {
                var Composicao = Composicoes.FirstOrDefault();

                ComposicoesAgrupadas = ComposicoesAgrupadas.Where(w => w.CnaeId == Composicao.CnaeId && w.ListaServicoId == Composicao.ListaServicoId).ToList();

                ComposicoesAgrupadasDescricao = ComposicoesAgrupadasDescricao.Where(w => w.CnaeId == Composicao.CnaeId && w.ListaServicoId == Composicao.ListaServicoId).ToList();
            }

            var CapaAutorizacaoNfse = new CapaAutorizacaoNfse();

            string json;

            List<NfeFaturamentoComposicaoEntity> NfeFaturamentoComposicaoList;

            StringBuilder descricaoConfiguracaoNfe = new StringBuilder();

            foreach (var agrupamento in ComposicoesAgrupadas)
            {
                descricaoConfiguracaoNfe = new StringBuilder();

                var aux = ComposicoesAgrupadasDescricao.Where(w => w.CnaeId == agrupamento.CnaeId && w.ListaServicoId == agrupamento.ListaServicoId).ToList();

                foreach (var item in aux)
                {
                    if (item.TipoDesconto != '\0')
                    {
                        descricaoConfiguracaoNfe.Append(item.DescricaoConfiguracaoNfe).Append(". QTD: ").Append(string.Format("{0:N2}", item.QuantidadeComposicao)).Append(". VALOR: R$ ").Append(string.Format("{0:N2}", item.ValorTipoComposicao)).Append(". DSCT: R$ ").Append(string.Format("{0:N2}", item.ValorDesconto)).Append(". TOT: R$ ").AppendLine(string.Format("{0:N2}", item.TotalComDesconto));
                    }
                    else
                    {
                        descricaoConfiguracaoNfe.Append(item.DescricaoConfiguracaoNfe).Append(". QTD: ").Append(string.Format("{0:N2}", item.QuantidadeComposicao)).Append(". VALOR: R$ ").Append(string.Format("{0:N2}", item.ValorTipoComposicao)).Append(". TOT: R$ ").AppendLine(string.Format("{0:N2}", item.TotalComDesconto));
                    }
                }

                #region Preenchimento da Entidade
                try
                {
                    CapaAutorizacaoNfse = new CapaAutorizacaoNfse
                    {
                        GrvId = grvId,

                        IdentificadorNota = new DetranController().GetDetranSequence("NFE"),

                        UsuarioId = usuarioId,

                        Homologacao = isDev,

                        Autorizacao = Autorizar(grv, Deposito, ClienteDeposito, NfeRegras, Empresa, Atendimento, agrupamento, descricaoConfiguracaoNfe.ToString().Trim(), isDev)
                    };
                }
                catch (Exception ex)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, ex.Message);

                    returnList.Add(ex.Message);

                    continue;
                }
                #endregion Preenchimento da Entidade


                #region Cadastro do Envio/Reenvio
                try
                {
                    if (Nfe.NfeId == 0)
                    {
                        // Cadastro do Envio
                        Nfe = CadastrarNfe(grvId, Empresa.Cnpj, CapaAutorizacaoNfse.IdentificadorNota, usuarioId);
                    }
                    else
                    {
                        // Cadastro do Reenvio
                        Nfe = CadastrarNfe(grvId, Empresa.Cnpj, CapaAutorizacaoNfse.IdentificadorNota, usuarioId, Nfe.NfeId);
                    }
                }
                catch (Exception ex)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, ex.Message);

                    returnList.Add("Erro ao cadastrar a NF: " + ex.Message);

                    continue;
                }

                Nfe.Cliente = Cliente.Nome;

                Nfe.Deposito = Deposito.Descricao;

                #endregion Cadastro do Envio/Reenvio


                #region Cadastro da Composição da Nota Fiscal

                NfeFaturamentoComposicaoList = new List<NfeFaturamentoComposicaoEntity>();

                try
                {
                    new NfeFaturamentoComposicaoController().Cadastrar(Nfe.NfeId, Composicoes.Where(w => w.Servico == agrupamento.Servico).ToList());
                }
                catch (Exception ex)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, ex.Message);

                    returnList.Add("Erro ao cadastrar a composição da NF: " + ex.Message);

                    continue;
                }
                #endregion Cadastro da Composição da Nota Fiscal


                #region Execução do Web Service
                try
                {
                    json = new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse);
                }
                catch (Exception ex)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, ex.Message);

                    returnList.Add("Erro na Execução do Web Service: " + ex.Message);

                    continue;
                }

                returnList.Add(json);
                #endregion Execução do Web Service


                #region Processamento do resultado
                try
                {
                    if (!ProcessarResultado(json, usuarioId, Nfe))
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, ex.Message);

                    returnList.Add("Erro no processamento do resultado do Web Service: " + ex.Message);

                    continue;
                }
                finally
                {
                    Nfe = new NfeEntity();
                }
                #endregion Processamento do resultado

                Nfe = new NfeEntity();
            }

            return returnList;
        }

        private NfeEntity CadastrarNfe(int grvId, string cnpj, string identificadorNota, int usuarioId, int nfeComplementarId = 0)
        {
            var Nfe = new NfeEntity
            {
                GrvId = grvId,

                Cnpj = cnpj,

                UsuarioCadastroId = usuarioId,

                IdentificadorNota = identificadorNota
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

        private Autorizacao Autorizar(GrvEntity grv, DepositoEntity deposito, ClienteDepositoEntity clienteDeposito, List<NfeRegraEntity> nfeRegras, EmpresaEntity empresa, AtendimentoEntity atendimento, NfeViewFaturamentoComposicaoAgrupadoEntity composicao, string descricaoConfiguracaoNfe, bool isDev)
        {
            var now = DateTime.Now.AddHours(-1);

            var Autorizacao = new Autorizacao
            {
                data_emissao = now.ToString("yyyy-MM-dd") + "T" + now.ToString("HH:mm:ss"),

                natureza_operacao = "1",

                optante_simples_nacional = empresa.OptanteSimplesNacional.Equals('S') ? "true" : "false",

                prestador = Prestador(empresa, composicao.FlagEnviarInscricaoEstadual),

                tomador = Tomador(deposito, atendimento)
            };

            Autorizacao.servico = Servico(grv, composicao, Autorizacao.prestador, clienteDeposito, nfeRegras, descricaoConfiguracaoNfe, isDev);

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

        private Tomador Tomador(DepositoEntity deposito, AtendimentoEntity atendimento)
        {
            return new Tomador
            {
                cpf = atendimento.NotaFiscalCpf.Length.Equals(11) ? atendimento.NotaFiscalCpf : string.Empty,

                cnpj = atendimento.NotaFiscalCpf.Length.Equals(14) ? atendimento.NotaFiscalCpf : string.Empty,

                razao_social = atendimento.NotaFiscalNome.Trim(),

                telefone = (atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone).Length.Equals(0) ? "2199999999" : atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone,

                email = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalEmail) ? atendimento.NotaFiscalEmail.Trim() : deposito.EmailNfe,

                endereco = Endereco(atendimento),

                inscricao_municipal = atendimento.NotaFiscalEmailInscricaoMunicipalTomadorServico
            };
        }

        private Endereco Endereco(AtendimentoEntity atendimento)
        {
            var CEP = new EnderecoCompletoController().Selecionar(atendimento.NotaFiscalCep);

            string CodigoMunicipioIbge;

            if (CEP != null && !string.IsNullOrWhiteSpace(CEP.CodigoMunicipioIbge))
            {
                CodigoMunicipioIbge = CEP.CodigoMunicipioIbge;
            }
            else
            {
                CodigoMunicipioIbge = new MunicipioController().SelecionarPrimeiroCodigoIbge(atendimento.NotaFiscalUf);
            }

            return new Endereco
            {
                logradouro = atendimento.NotaFiscalEndereco.Trim(),

                numero = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalNumero) && atendimento.NotaFiscalNumero.Length > 10 ? atendimento.NotaFiscalNumero.Substring(0, 10) : atendimento.NotaFiscalNumero,

                complemento = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalComplemento) ? atendimento.NotaFiscalComplemento.Trim() : "...",

                bairro = atendimento.NotaFiscalBairro.Trim(),

                uf = atendimento.NotaFiscalUf,

                cep = atendimento.NotaFiscalCep,

                codigo_municipio = CodigoMunicipioIbge
            };
        }

        private Servico Servico(GrvEntity grv, NfeViewFaturamentoComposicaoAgrupadoEntity composicao, Prestador prestador, ClienteDepositoEntity clienteDeposito, List<NfeRegraEntity> nfeRegras, string descricaoConfiguracaoNfe, bool isDev)
        {
            CnaeListaServicoParametroMunicipioEntity CnaeListaServicoParametroMunicipio = new CnaeListaServicoParametroMunicipioEntity
            {
                CnaeId = composicao.CnaeId,

                ListaServicoId = composicao.ListaServicoId,

                CodigoMunicipioIbge = prestador.codigo_municipio
            };

            if ((CnaeListaServicoParametroMunicipio = new CnaeListaServicoParametroMunicipioController().Selecionar(CnaeListaServicoParametroMunicipio)) == null)
            {
                throw new Exception("Associação entre CNAE, Lista de Serviço e Município inexistente");
            }

            decimal valorIss = 0;

            decimal AliquotaIss = 0;

            GravarLog("");
            GravarLog($"GRV {grv.NumeroFormularioGrv} ({(isDev ? "DEV" : "PROD")})");
            GravarLog("");

            composicao.TotalComDesconto = Math.Round(composicao.TotalComDesconto, 2);

            if (PossuiRegraNfe(nfeRegras, "SEMALIQUOTA"))
            {
                GravarLog("R1: POSSUI REGRA 'SEMALIQUOTA'");

                AliquotaIss = 0;
            }
            else if (PossuiRegraNfe(nfeRegras, "CODTRIBMUN_0000"))
            {
                GravarLog("R2: POSSUI REGRA 'CODTRIBMUN_0000'");

                CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio = "0000";
            }
            else if (clienteDeposito.AliquotaIss > 0)
            {
                GravarLog($"R3: CLIDEP POSSUI ALIQUOTA ISS > 0: {clienteDeposito.AliquotaIss}");

                AliquotaIss = clienteDeposito.AliquotaIss;
            }
            else
            {
                GravarLog($"R4: CNAE LISTA SERVICO PARAMETRO MUNICIPIO ALIQUOTA ISS: {CnaeListaServicoParametroMunicipio.AliquotaIss.Value}");

                AliquotaIss = CnaeListaServicoParametroMunicipio.AliquotaIss.Value;
            }

            if (composicao.FlagEnviarValorIss == 'S')
            {
                GravarLog("R5: POSSUI FLAG ENVIAR VALOR ISS");

                if (clienteDeposito.FlagValorIssIgualProdutoBaseCalculoAliquota == 'S')
                {
                    GravarLog("R5.1: POSSUI FLAG VALOR ISS IGUAL PRODUTO BASE CALCULO ALIQUOTA:");
                    GravarLog("    ValorIss: (Composição * AliquotaIss) / 100");
                    GravarLog($"    ValorIss: {(composicao.TotalComDesconto * AliquotaIss) / 100}");

                    valorIss = (composicao.TotalComDesconto * AliquotaIss) / 100;
                }
                else
                {
                    GravarLog($"R5.2: AliquotaIss / 100 = {AliquotaIss / 100}");
                    GravarLog("    ValorIss = AliquotaIss / 100:");
                    GravarLog($"    ValorIss = {AliquotaIss / 100}");

                    valorIss = AliquotaIss / 100;
                }
            }

            if (PossuiRegraNfe(nfeRegras, "VALOR_ISS_TRUNCAR"))
            {
                GravarLog("R6: POSSUI REGRA 'VALOR_ISS_TRUNCAR'");

                GravarLog($"    ValorIss = {Math.Truncate(100 * valorIss) / 100}");

                valorIss = Math.Truncate(100 * valorIss) / 100;
            }

            //else
            //{
            //    var regra = nfeRegras.Where(w => w.RegraCodigo.Equals("VLSERVICO=TOTAL+IMP") && w.Ativo == 1)
            //        .FirstOrDefault();

            //    if (regra != null)
            //    {
            //        valorServicos = Math.Round(composicao.TotalComDesconto + valorIss, 2).ToString().Replace(",", ".");
            //    }
            //    else
            //    {
            //        valorServicos = Math.Round(composicao.TotalComDesconto, 2).ToString().Replace(",", ".");
            //    }
            //}

            string valorServicos;

            if (isDev)
            {
                valorServicos = "1";
            }
            else
            {
                valorServicos = composicao.TotalComDesconto.ToString().Replace(",", ".");
            }

            string baseCalculo = string.Empty;

            if (PossuiRegraNfe(nfeRegras, "BASE_CALCULO"))
            {
                GravarLog("R7: POSSUI REGRA 'BASE_CALCULO'");

                GravarLog("R7.1: baseCalculo = valorServicos");
                GravarLog($"      baseCalculo = {valorServicos}");

                baseCalculo = valorServicos;
            }

            GravarLog($"F1: COMPOSIÇÃO DO FATURAMENTO (ARREDONDAMENTO EM DUAS CASAS DECIMAIS): {composicao.TotalComDesconto} >>> {Math.Round(composicao.TotalComDesconto, 2)}");

            GravarLog($"F2: ALÍQUOTA ISS (ARREDONDAMENTO EM DUAS CASAS DECIMAIS): {string.Format("{0:N2}", AliquotaIss).Replace(",", ".")}");

            GravarLog($"F3: DISCRIMINAÇÃO: {descricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv}");

            GravarLog($"F4: CÓDIGO CNAE: {composicao.Cnae}");

            GravarLog($"F5: ITEM DA LISTA SERVIÇO: {CnaeListaServicoParametroMunicipio.ListaServico}");

            GravarLog($"F6: VALOR ISS (ARREDONDAMENTO EM DUAS CASAS DECIMAIS): {string.Format("{0:N2}", valorIss).Replace(",", ".")}");

            GravarLog($"F7: CÓDIGO TRIBUTÁRIO MUNICÍPIO: {(!string.IsNullOrWhiteSpace(CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio) ? CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio : "CNAE + LISTASERVICO NÃO ASSOCIADO AO MUNICÍPIO")}");

            GravarLog($"F8: VALOR DOS SERVIÇOS: {valorServicos}. 1 SE FOR AMBIENTE DE DESENVOLVIMENTO");

            GravarLog($"F9: BASE DE CÁLCULO: {(!string.IsNullOrWhiteSpace(baseCalculo) ? baseCalculo : "CLIDEP NÃO POSSUI A REGRA 'BASE_CALCULO'")}");

            return new Servico
            {
                aliquota = string.Format("{0:N2}", AliquotaIss).Replace(",", "."),

                discriminacao = descricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv,

                iss_retido = "false",

                codigo_cnae = composicao.Cnae,

                item_lista_servico = CnaeListaServicoParametroMunicipio.ListaServico,

                valor_iss = string.Format("{0:N2}", valorIss).Replace(",", "."),

                codigo_tributario_municipio = CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio,

                valor_servicos = valorServicos,

                base_calculo = !string.IsNullOrWhiteSpace(baseCalculo) ? baseCalculo : null,
            };
        }

        private void GravarLog(string message)
        {
            string drive = new Tools().DriveToSave();

            if (!Directory.Exists($@"{drive}Sistemas\GeradorNF\NFE"))
            {
                Directory.CreateDirectory($@"{drive}Sistemas\GeradorNF\NFE");
            }

            using (StreamWriter sw = new StreamWriter($@"{drive}Sistemas\GeradorNF\NFE\NfeGerarNotaFiscalController.log", true, Encoding.UTF8))
            {
                sw.WriteLine(message);
            }
        }

        private bool PossuiRegraNfe(List<NfeRegraEntity> nfeRegras, string codigoRegra)
        {
            if (nfeRegras == null || nfeRegras.Count == 0)
            {
                return false;
            }

            return nfeRegras.Any(w => w.RegraCodigo.Equals(codigoRegra) && w.Ativo == 1);
        }
    }
}