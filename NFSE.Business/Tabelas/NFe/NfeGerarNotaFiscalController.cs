using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Domain.Entities;
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
            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            // Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            DataBase.SetContextInfo(usuarioId);

            var acao = Acao.Solicitação;

            #region NFe
            var Nfe = new NfeEntity();

            if ((Nfe = new NfeController().Selecionar(grvId)) != null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "GRV já possui Nota Fiscal cadastrada");

                throw new Exception("GRV já possui Nota Fiscal cadastrada");
            }
            #endregion NFe

            #region GRV
            var grv = new GrvController().Selecionar(grvId);
            #endregion Grv

            #region Depósito
            var Deposito = new DepositoController().Selecionar(grv.DepositoId);
            #endregion Depósito

            #region Regras do Faturamento
            if (new RegraFaturamentoController().Selecionar(grv.ClienteId, grv.DepositoId, new TipoRegraFaturamentoController().Selecionar("NFE").FaturamentoRegraTipoId) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "CLIDEP sem regra de NFE definido");

                throw new Exception("CLIDEP sem regra de NFE definido");
            }
            #endregion Regras do Faturamento

            #region Empresa
            var Empresa = new EmpresaEntity();

            if ((Empresa = new EmpresaController().Selecionar(Deposito.EmpresaId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            #region Atendimento
            var Atendimento = new AtendimentoEntity();

            if ((Atendimento = new AtendimentoController().Selecionar(grvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Atendimento não encontrado");

                throw new Exception("Atendimento não encontrado");
            }
            #endregion Atendimento

            #region Faturamento
            var Faturamentos = new List<FaturamentoEntity>();

            if ((Faturamentos = new FaturamentoController().Listar(Atendimento.AtendimentoId, 'P')) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Faturamento não encontrado");

                throw new Exception("Faturamento não encontrado");
            }

            if (Faturamentos.Sum(c => c.ValorPagamento).Equals(0))
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Faturamento sem valor");

                throw new Exception("Faturamento sem valor");
            }
            #endregion Faturamento

            #region Valores somados da Composição do Faturamento
            var Composicoes = new List<FaturamentoAssociadoCnaeEntity>();

            if ((Composicoes = new FaturamentoAssociadoCnaeController().Listar(grv.GrvId)) == null)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

                throw new Exception("Composição do Faturamento não encontrado");
            }
            #endregion Valores somados da Composição do Faturamento

            // TODO: Ver se devemos retirar as composições que não possuem CNAE nem ListaServico
            Composicoes = Composicoes.Where(w => w.CnaeId > 0 && w.ListaServicoId > 0).ToList();

            if (Composicoes.Count == 0)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");

                throw new Exception("Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");
            }

            var CapaAutorizacaoNfse = new CapaAutorizacaoNfse();

            var jsonList = new List<string>();

            string json;

            foreach (var composicao in Composicoes)
            {
                #region Preenchimento do Entityo
                CapaAutorizacaoNfse = new CapaAutorizacaoNfse
                {
                    GrvId = grvId,

                    IdentificadorNota = new DetranController().GetDetranSequence("NFE"),

                    UsuarioId = usuarioId,

                    Homologacao = isDev,

                    Autorizacao = Autorizar(grv, Deposito, Empresa, Atendimento, composicao, isDev)
                };
                #endregion Preenchimento do Entityo

                // Cadastro do Envio
                Nfe = CadastrarEnvio(grvId, Empresa.Cnpj, 'E', CapaAutorizacaoNfse.IdentificadorNota, usuarioId);

                json = new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse);

                // Execuão do Web Service
                try
                {
                    ExecutarWebService(json, usuarioId, Nfe, 'E');
                }
                catch (Exception ex)
                {
                    continue;
                }

                jsonList.Add(json);
            }

            return jsonList;
        }

        private void ExecutarWebService(string json, int usuarioId, NfeEntity nfe, char tipo)
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

                        Tipo = tipo,

                        Mensagem = (string)JObject.Parse(json)["status"]
                    });

                    nfe.Status = 'A';

                    new NfeController().AguardandoProcessamento(nfe);
                }
                else
                {
                    var retornoErro = new NfeWsErroModel
                    {
                        GrvId = nfe.GrvId,
                        IdentificadorNota = nfe.IdentificadorNota.Value,
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
                }
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                new NfeController().Atualizar(nfe);
            }
            catch (Exception ex)
            {
                // throw;
            }
        }

        private NfeEntity CadastrarEnvio(int grvId, string cnpj, char tipo, int identificadorNota, int usuarioId)
        {
            var Nfe = new NfeEntity();

            if (tipo.Equals('E'))
            {
                try
                {
                    Nfe.NfeId = new NfeController().Cadastrar(new NfeEntity
                    {
                        GrvId = grvId,

                        Cnpj = cnpj,

                        UsuarioCadastroId = usuarioId,

                        IdentificadorNota = identificadorNota
                    });
                }
                catch (Exception ex)
                {
                    if (true)
                    {

                    }
                }

                Nfe = new NfeController().Selecionar(grvId);
            }
            else
            {
                var Nfes = new NfeController().Listar(grvId);

                if (Nfes != null)
                {
                    Nfe = Nfes.OrderByDescending(m => m.DataCadastro).FirstOrDefault();
                }
                else
                {
                    throw new Exception("Cadastro da Nfe no status R não encontrado");
                }
            }

            return Nfe;
        }

        private Autorizacao Autorizar(GrvEntity grv, DepositoEntity deposito, EmpresaEntity empresa, AtendimentoEntity atendimento, FaturamentoAssociadoCnaeEntity composicao, bool isDev)
        {
            var Now = DateTime.Now;

            var Autorizacao = new Autorizacao
            {
                data_emissao = Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("HH:mm:ss"),

                natureza_operacao = "1",

                optante_simples_nacional = "false",

                prestador = Prestador(empresa),

                tomador = Tomador(grv, deposito, atendimento)
            };

            Autorizacao.servico = Servico(grv, atendimento, empresa, composicao, Autorizacao.prestador, isDev);

            return Autorizacao;
        }

        private Prestador Prestador(EmpresaEntity empresa)
        {
            return new Prestador
            {
                cnpj = empresa.Cnpj,

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

        private Servico Servico(GrvEntity grv, AtendimentoEntity atendimento, EmpresaEntity empresa, FaturamentoAssociadoCnaeEntity composicao, Prestador prestador, bool isDev)
        {
            if (composicao.CnaeId == null || composicao.CnaeId.Equals(0))
            {
                throw new Exception("Código CNAE não associado");
            }

            if (composicao.ListaServicoId == null || composicao.ListaServicoId.Equals(0))
            {
                throw new Exception("Lista de Serviço não associado");
            }

            #region Lista de Serviço
            CnaeListaServicoEntity ListaServico;

            if ((ListaServico = new CnaeListaServicoController().Selecionar(new CnaeListaServicoEntity { CnaeId = composicao.CnaeId.Value, ListaServicoId = composicao.ListaServicoId.Value })) == null)
            {
                throw new Exception("Lista de Serviço inexistente");
            }

            if (string.IsNullOrWhiteSpace(ListaServico.ListaServico))
            {
                throw new Exception("Lista de Serviço sem informação do Item da Lista de Serviço");
            }
            #endregion Lista de Serviço

            #region Lista de Serviço
            ParametroMunicipioEntity ParametroMunicipio;

            if ((ParametroMunicipio = new ParametroMunicipioController().Selecionar(new ParametroMunicipioEntity { CodigoCnae = composicao.Cnae.ToString(), CodigoMunicipioIbge = prestador.codigo_municipio })) == null)
            {
                throw new Exception("Parâmetro do Município inexistente");
            }

            if (string.IsNullOrWhiteSpace(ListaServico.ListaServico))
            {
                throw new Exception("Lista de Serviço sem informação do Item da Lista de Serviço");
            }
            #endregion Lista de Serviço

            return new Servico
            {
                aliquota = string.Format("{0:N2}", ListaServico.AliquotaIss).Replace(",", "."),

                discriminacao = composicao.DescricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv,

                iss_retido = "false",

                codigo_cnae = composicao.Cnae,

                item_lista_servico = ListaServico.ListaServico,

                valor_iss = composicao.FlagEnviarValorIss == 'S' ? string.Format("{0:N2}", ListaServico.AliquotaIss / 100).Replace(",", ".") : "0",

                // codigo_tributario_municipio = empresa.CodigoTributarioMunicipio > 0 ? empresa.CodigoTributarioMunicipio.Value.ToString("0000") : string.Empty,

                codigo_tributario_municipio = ParametroMunicipio.CodigoTributarioMunicipio,

                valor_servicos = isDev ? "1" : Math.Round(composicao.TotalComDesconto, 2).ToString().Replace(",", ".")
            };
        }
    }
}
