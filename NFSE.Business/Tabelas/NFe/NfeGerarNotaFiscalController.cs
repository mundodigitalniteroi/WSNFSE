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
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeGerarNotaFiscalController
    {
        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, 0, usuarioId, isDev);
        }

        public List<string> GerarNovaNotaFiscal(int grvId, int identificadorNota, int usuarioId, bool isDev)
        {
            return GerarNotaFiscal(grvId, identificadorNota, usuarioId, isDev);
        }

        private List<string> GerarNotaFiscal(int grvId, int identificadorNota, int usuarioId, bool isDev)
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
            //   M: Cadastro Manual

            var Nfe = new NfeEntity
            {
                GrvId = grvId,

                IdentificadorNota = identificadorNota
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
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }
            #endregion Valores somados da Composição do Faturamento

            #region Valores somados da Composição do Faturamento
            var ComposicoesAgrupadasDescricao = new List<NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity>();

            if ((ComposicoesAgrupadasDescricao = new NfeViewFaturamentoComposicaoAgrupadoDescricaoController().Listar(grvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

                returnList.Add("AVISO: Composição do Faturamento não encontrado");

                return returnList;
            }
            #endregion Valores somados da Composição do Faturamento

            #region Composições
            var Composicoes = new List<NfeViewFaturamentoComposicaoEntity>();

            if ((Composicoes = new NfeViewFaturamentoComposicaoController().Listar(grvId, Nfe.NfeId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

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
                        descricaoConfiguracaoNfe.AppendLine($"{item.DescricaoConfiguracaoNfe}. QTD: {string.Format("{0:N2}", item.QuantidadeComposicao)}. VALOR: R$ {string.Format("{0:N2}", item.ValorTipoComposicao)}. DSCT: R$ {string.Format("{0:N2}", item.ValorDesconto)}. TOT: R$ {string.Format("{0:N2}", item.TotalComDesconto)}");
                    }
                    else
                    {
                        descricaoConfiguracaoNfe.AppendLine($"{item.DescricaoConfiguracaoNfe}. QTD: {string.Format("{0:N2}", item.QuantidadeComposicao)}. VALOR: R$ {string.Format("{0:N2}", item.ValorTipoComposicao)}. TOT: R$ {string.Format("{0:N2}", item.TotalComDesconto)}");
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
            }

            return returnList;
        }

        private NfeEntity CadastrarNfe(int grvId, string cnpj, int identificadorNota, int usuarioId, int nfeComplementarId = 0)
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
            var Now = DateTime.Now;

            var Autorizacao = new Autorizacao
            {
                data_emissao = Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("HH:mm:ss"),

                natureza_operacao = "1",

                optante_simples_nacional = "false",

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

                razao_social = atendimento.NotaFiscalNome,

                telefone = (atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone).Length.Equals(0) ? "2199999999" : atendimento.NotaFiscalDdd + atendimento.NotaFiscalTelefone,

                email = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalEmail) ? atendimento.NotaFiscalEmail : deposito.EmailNfe,

                endereco = Endereco(atendimento)
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
                logradouro = atendimento.NotaFiscalEndereco,

                numero = atendimento.NotaFiscalNumero,

                complemento = !string.IsNullOrWhiteSpace(atendimento.NotaFiscalComplemento) ? atendimento.NotaFiscalComplemento : "...",

                bairro = atendimento.NotaFiscalBairro,

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

            decimal AliquotaIss;

            if (nfeRegras != null && nfeRegras.Where(w => w.RegraCodigo.Equals("SEMALIQUOTA")).Count() > 0)
            {
                AliquotaIss = 0;
            }
            else if (clienteDeposito.AliquotaIss > 0)
            {
                AliquotaIss = clienteDeposito.AliquotaIss;
            }
            else
            {
                AliquotaIss = CnaeListaServicoParametroMunicipio.AliquotaIss.Value;
            }

            if (composicao.FlagEnviarValorIss == 'S')
            {
                if (clienteDeposito.FlagValorIssIgualProdutoBaseCalculoAliquota == 'S')
                {
                    valorIss = (Math.Round(composicao.TotalComDesconto, 2) * AliquotaIss) / 100;
                }
                else
                {
                    valorIss = AliquotaIss / 100;
                }
            }

            string valorServicos;
            
            if (isDev)
            {
                valorServicos = "1";
            }
            else
            {
                valorServicos = Math.Round(composicao.TotalComDesconto, 2)
                    .ToString()
                    .Replace(",", ".");
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

            return new Servico
            {
                aliquota = string.Format("{0:N2}", AliquotaIss).Replace(",", "."),

                discriminacao = descricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv,

                iss_retido = "false",

                codigo_cnae = composicao.Cnae,

                item_lista_servico = CnaeListaServicoParametroMunicipio.ListaServico,

                valor_iss = string.Format("{0:N2}", valorIss).Replace(",", "."),

                codigo_tributario_municipio = CnaeListaServicoParametroMunicipio.CodigoTributarioMunicipio,

                valor_servicos = valorServicos
            };
        }
    }
}