using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFSE.Business.Tabelas;
using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Tabelas.NFe;
using NFSE.Business.Util;
using NFSE.Domain.Entities;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;

namespace NFSE.Business
{
    public class Main
    {
        public string SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Solicitação);

            var prestadorAcesso = ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.Autorizacao.prestador.cnpj, Acao.Solicitação, nfe, model);

            string uri = prestadorAcesso.server + "?ref=" + model.IdentificadorNota;

            model.Autorizacao.servico.codigo_tributario_municipio = !prestadorAcesso.codigo_tributario_municipio.Equals(string.Empty) ? prestadorAcesso.codigo_tributario_municipio : null;
            model.Autorizacao.servico.item_lista_servico = prestadorAcesso.item_lista_servico;
            model.Autorizacao.servico.codigo_cnae = prestadorAcesso.codigo_cnae;

            var tools = new Tools();

            string json = tools.ObjToJSON(model.Autorizacao);

            string resposta;

            try
            {
                resposta = tools.PostNfse(uri, json, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Solicitação, "Ocorreu um erro ao solicitar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                new AutorizacaoNotaFiscal().Cadastrar(prestadorAcesso, model, resposta);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(model.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Solicitação, "Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            return resposta;
        }

        //public string ReceberNotaFiscal(Consultar model)
        //{
        //    return new JavaScriptSerializer().Serialize(Consultar_obj(model));
        //}

        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Retorno);

            var prestadorAcesso = ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.CnpjPrestador, Acao.Retorno, nfe);

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "/" + model.IdentificadorNota, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return CadastrarNotaFiscalRecebida(model, nfse);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity CadastrarNotaFiscalRecebida(Consulta notaFiscalRecebida, string retorno)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retorno);

            if (retornoConsulta.Status.Trim().ToUpper().Contains("PROCESSANDO_AUTORIZACAO"))
            {
                return retornoConsulta;
            }

            if (retornoConsulta.url == null)
            {
                retorno = retorno.Replace("\"codigo\"", "\"CodigoErro\"");
                retorno = retorno.Replace("\"mensagem\"", "\"MensagemErro\"");
                retorno = retorno.Replace("\"correcao\"", "\"CorrecaoErro\"");
                retorno = retorno.Replace("[", "");
                retorno = retorno.Replace("]", "");

                var retornoErro = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<NfeWsErroModel>(retorno);

                JObject jsonErro = JObject.Parse(retorno);

                if ((string)jsonErro.SelectToken("erros.CodigoErro") != null)
                {
                    retornoErro.CodigoErro = ((string)jsonErro.SelectToken("erros.CodigoErro")).Trim().ToUpper();
                }

                if ((string)jsonErro.SelectToken("erros.MensagemErro") != null)
                {
                    retornoErro.MensagemErro = ((string)jsonErro.SelectToken("erros.MensagemErro")).Trim();
                }

                if ((string)jsonErro.SelectToken("erros.CorrecaoErro") != null)
                {
                    retornoErro.CorrecaoErro = ((string)jsonErro.SelectToken("erros.CorrecaoErro")).Trim();
                }

                retornoErro.IdentificadorNota = notaFiscalRecebida.IdentificadorNota;
                retornoErro.UsuarioId = notaFiscalRecebida.UsuarioId;
                retornoErro.Acao = (char)Acao.Retorno;
                retornoErro.OrigemErro = (char)OrigemErro.WebService;
                retornoErro.Status = retornoErro.Status.Trim().ToUpper();

                retornoErro.ErroId = new NfeWsErroController().Cadastrar(retornoErro);

                retornoErro = new NfeWsErroController().Selecionar(new NfeWsErroModel { ErroId = retornoErro.ErroId } );

                retornoConsulta.ErroId = retornoErro.ErroId;
                retornoConsulta.IdentificadorNota = retornoErro.IdentificadorNota;
                retornoConsulta.UsuarioId = retornoErro.UsuarioId;
                retornoConsulta.Acao = retornoErro.Acao;
                retornoConsulta.OrigemErro = retornoErro.OrigemErro;
                retornoConsulta.Status = retornoErro.Status;
                retornoConsulta.CodigoErro = retornoErro.CodigoErro;
                retornoConsulta.MensagemErro = retornoErro.MensagemErro;
                retornoConsulta.CorrecaoErro = retornoErro.CorrecaoErro;
                retornoConsulta.DataHoraCadastro = retornoErro.DataHoraCadastro;

                return retornoConsulta;
            }

            var notaFiscal = new NfeRetornoModel
            {
                AutorizacaoNotaFiscalId = notaFiscalRecebida.IdentificadorNota,
                UsuarioId = notaFiscalRecebida.UsuarioId,
                StatusNotaFiscal = retornoConsulta.Status.ToUpper(),
                NumeroNotaFiscal = retornoConsulta.numero,
                CodigoVerificacao = retornoConsulta.codigo_verificacao.Trim(),
                DataEmissao = retornoConsulta.data_emissao,
                UrlNotaFiscal = retornoConsulta.url,
                CaminhoNotaFiscal = retornoConsulta.caminho_xml_nota_fiscal
            };

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                using (var memoryStream = new MemoryStream())
                {
                    notaFiscal.UrlNotaFiscal = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                    new Tools().ObterImagemEndereco(notaFiscal.UrlNotaFiscal).Save(memoryStream, ImageFormat.Jpeg);

                    notaFiscal.ImagemNotaFiscal = memoryStream.ToArray();
                }

                retornoConsulta.ImagemNotaFiscal = notaFiscal.ImagemNotaFiscal;
            }

            retornoConsulta.NotaFiscalId = new NfeRetornoController().Cadastrar(notaFiscal);

            return retornoConsulta;
        }

        public string CancelarNotaFiscal(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(model.GrvId, model.UsuarioId, model.IdentificadorNota, Acao.Cancelamento);

            var prestadorAcesso = ConsultarPrestadorServico(model.GrvId, model.UsuarioId, model.CnpjPrestador, Acao.Cancelamento, nfe);

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
                return tools.CancelarNfse(prestadorAcesso.server + "/" + model.IdentificadorNota, json, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(nfe.GrvId, model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Cancelamento, "Ocorreu um erro cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private void CadastrarErroGenerico(int grvId, int usuarioId, int? identificadorNota, OrigemErro origemErro, Acao acao, string mensagemErro)
        {
            var erro = new NfeWsErroModel
            {
                GrvId = grvId,
                IdentificadorNota = identificadorNota,
                UsuarioId = usuarioId,
                Acao = (char)acao,
                OrigemErro = (char)origemErro,
                MensagemErro = mensagemErro
            };

            try
            {
                new NfeWsErroController().Cadastrar(erro);
            }
            catch { }
        }

        private NfeEntity ConsultarNotaFiscal(int grvId, int usuarioId, int identificadorNota, Acao acao)
        {
            List<NfeEntity> nfe;

            try
            {
                if ((nfe = new NfeController().ListarPorIdentificadorNota(identificadorNota)) == null)
                {
                    CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Nota Fiscal não encontrada no cadastro do Depósito Público");

                    throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público (" + identificadorNota + ")");
                }
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            return nfe.FirstOrDefault();
        }

        private PrestadorServico ConsultarPrestadorServico(int grvId, int usuarioId, string cnpj, Acao acao, NfeEntity nfe, CapaAutorizacaoNfse capaAutorizacaoNfse = null)
        {
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            cnpj = cnpj.Replace("/", "").Replace(".", "").Replace("-", "");

            var prestadorAcesso = new PrestadorServico();

            try
            {
                prestadorAcesso.server = new Server().GetRemoteServer();
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar os dados do Servidor: " + ex);

                throw new Exception("Ocorreu um erro ao consultar os dados do Servidor: " + ex);
            }

            try
            {
                using (var dtPrestador = new PrestadorController().Consultar(cnpj, capaAutorizacaoNfse))
                {
                    if (dtPrestador == null)
                    {
                        CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Prestador de Serviços não configurado. CNPJ: " + cnpj);

                        throw new Exception("Prestador de Serviços não configurado. CNPJ: " + cnpj);
                    }

                    foreach (DataRow row in (InternalDataCollectionBase)dtPrestador.Rows)
                    {
                        prestadorAcesso.id_nfse_prestador = row["id_nfse_prestador"].ToString();
                        prestadorAcesso.prestador_cnpj = row["prestador_cnpj"].ToString();
                        prestadorAcesso.prestador_nome = row["prestador_nome"].ToString();
                        prestadorAcesso.prestador_inscricao_municipal = row["prestador_inscricao_municipal"].ToString();
                        prestadorAcesso.prestador_codigo_municipio_ibge = row["prestador_codigo_municipio_ibge"].ToString();
                        prestadorAcesso.prestador_chave = row["prestador_chave"].ToString();
                        prestadorAcesso.item_lista_servico = row["item_lista_servico"].ToString();
                        prestadorAcesso.codigo_tributario_municipio = row["codigo_tributario_municipio"].ToString();
                        prestadorAcesso.codigo_cnae = row["codigo_cnae"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);

                throw new Exception("Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);
            }

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);

                throw new Exception("Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);
            }

            return prestadorAcesso;
        }

        public List<string> GerarNotaFiscal(int grvId, int usuarioId, bool isDev)
        {
            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            // Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            DataBase.SetContextInfo(usuarioId);

            var acao = Acao.Solicitação;

            #region GRV
            var grv = new GrvEntity();

            if ((grv = new GrvController().Selecionar(grvId)) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "GRV não encontrado");

                throw new Exception("GRV não encontrado");
            }
            #endregion Grv

            #region Depósito
            var Deposito = new DepositoEntity();

            if ((Deposito = new DepositoController().Selecionar(grv.DepositoId)) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Depósito não encontrado");

                throw new Exception("Depósito não encontrado");
            }
            #endregion Depósito

            #region Regras do Faturamento
            if (new RegraFaturamentoController().Selecionar(grv.ClienteId, grv.DepositoId, new TipoRegraFaturamentoController().Selecionar("NFE").FaturamentoRegraTipoId) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "CLIDEP sem regra de NFE definido");

                throw new Exception("CLIDEP sem regra de NFE definido");
            }
            #endregion Regras do Faturamento

            #region Empresa
            var Empresa = new EmpresaEntity();

            if ((Empresa = new EmpresaController().Selecionar(Deposito.EmpresaId)) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Empresa associada não encontrada");

                throw new Exception("Empresa associada não encontrada");
            }
            #endregion Empresa

            #region Atendimento
            var Atendimento = new AtendimentoEntity();

            if ((Atendimento = new AtendimentoController().Selecionar(grvId)) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Atendimento não encontrado");

                throw new Exception("Atendimento não encontrado");
            }
            #endregion Atendimento

            #region Faturamento
            var Faturamentos = new List<FaturamentoEntity>();

            if ((Faturamentos = new FaturamentoController().Listar(Atendimento.AtendimentoId, 'P')) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Faturamento não encontrado");

                throw new Exception("Faturamento não encontrado");
            }

            if (Faturamentos.Sum(c => c.ValorPagamento).Equals(0))
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Faturamento sem valor");

                throw new Exception("Faturamento sem valor");
            }
            #endregion Faturamento

            #region Valores somados da Composição do Faturamento
            var Composicoes = new List<FaturamentoAssociadoCnaeEntity>();

            if ((Composicoes = new FaturamentoAssociadoCnaeController().Listar(grv.GrvId)) == null)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Composição do Faturamento não encontrado");

                throw new Exception("Composição do Faturamento não encontrado");
            }
            #endregion Valores somados da Composição do Faturamento

            // TODO: Ver se devemos retirar as composições que não possuem CNAE nem ListaServico
            Composicoes = Composicoes.Where(w => w.CnaeId > 0 && w.ListaServicoId > 0).ToList();

            if (Composicoes.Count == 0)
            {
                CadastrarErroGenerico(grvId, usuarioId, null, OrigemErro.MobLink, acao, "Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");

                throw new Exception("Composição do Faturamento sem Cnae e Lista de Serviço cadastrado");
            }

            var CapaAutorizacaoNfse = new CapaAutorizacaoNfse();

            var Nfe = new NfeEntity();

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

                json = SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse);

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
                Nfe.NfeId = new NfeController().Cadastrar(new NfeEntity
                {
                    GrvId = grvId,

                    Cnpj = cnpj,

                    UsuarioCadastroId = usuarioId,

                    IdentificadorNota = identificadorNota
                });

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

            return new Autorizacao
            {
                data_emissao = Now.ToString("yyyy-MM-dd") + "T" + Now.ToString("hh:mm:ss"),

                natureza_operacao = "1",

                optante_simples_nacional = "false",

                prestador = Prestador(empresa),

                tomador = Tomador(grv, deposito, atendimento),

                servico = Servico(grv, atendimento, empresa, composicao, isDev)
            };
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

        private Servico Servico(GrvEntity grv, AtendimentoEntity atendimento, EmpresaEntity empresa, FaturamentoAssociadoCnaeEntity composicao, bool isDev)
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

            try
            {
                if ((ListaServico = new CnaeListaServicoController().Selecionar(new CnaeListaServicoEntity { ListaServicoId = composicao.ListaServicoId.Value })) == null)
                {
                    throw new Exception("Lista de Serviço inexistente");
                }
            }
            catch (Exception)
            {
                throw;
            }
            #endregion Lista de Serviço

            return new Servico
            {
                aliquota = string.Format("{0:N2}", ListaServico.AliquotaIss).Replace(",", "."),

                discriminacao = composicao.DescricaoConfiguracaoNfe + " CONFORME PROCESSO " + grv.NumeroFormularioGrv,

                iss_retido = "false",

                codigo_cnae = composicao.Cnae,

                item_lista_servico = ListaServico.ListaServico.Replace(".", ""),

                valor_iss = composicao.FlagEnviarValorIss == 'S' ? string.Format("{0:N2}", ListaServico.AliquotaIss / 100).Replace(",", ".") : "0",

                codigo_tributario_municipio = empresa.CodigoTributarioMunicipio > 0 ? empresa.CodigoTributarioMunicipio.Value.ToString("0000") : string.Empty,

                valor_servicos = isDev ? "1" : Math.Round(composicao.TotalComDesconto, 2).ToString().Replace(",", ".")
            };
        }
    }
}