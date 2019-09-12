using Newtonsoft.Json.Linq;
using NFSE.Business.Util;
using NFSE.Domain.Entities;
using NFSE.Domain.Entities.DP;
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
        public string SolicitarEmissaoNotaFiscal(CapaAutorizacaoNfse capaAutorizacaoNfse)
        {
            DataBase.SystemEnvironment = capaAutorizacaoNfse.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(capaAutorizacaoNfse.UsuarioId, capaAutorizacaoNfse.IdentificadorNota, Acao.Solicitação);

            var prestadorAcesso = ConsultarPrestadorServico(capaAutorizacaoNfse.UsuarioId, capaAutorizacaoNfse.Autorizacao.prestador.cnpj, Acao.Solicitação, nfe, capaAutorizacaoNfse);

            string uri = prestadorAcesso.server + "?ref=" + capaAutorizacaoNfse.IdentificadorNota;

            capaAutorizacaoNfse.Autorizacao.servico.codigo_tributario_municipio = !prestadorAcesso.codigo_tributario_municipio.Equals(string.Empty) ? prestadorAcesso.codigo_tributario_municipio : null;
            capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico = prestadorAcesso.item_lista_servico;
            capaAutorizacaoNfse.Autorizacao.servico.codigo_cnae = prestadorAcesso.codigo_cnae;

            var tools = new Tools();

            string json = tools.ObjToJSON(capaAutorizacaoNfse.Autorizacao);

            string resposta;

            try
            {
                resposta = tools.PostNfse(uri, json, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(capaAutorizacaoNfse.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Solicitação, "Ocorreu um erro ao solicitar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + capaAutorizacaoNfse.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                new Tabelas.AutorizacaoNotaFiscal().Cadastrar(prestadorAcesso, capaAutorizacaoNfse, resposta);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(capaAutorizacaoNfse.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Solicitação, "Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal (" + capaAutorizacaoNfse.IdentificadorNota + "): " + ex.Message);
            }

            return resposta;
        }

        //public string ReceberNotaFiscal(Consultar model)
        //{
        //    return new JavaScriptSerializer().Serialize(Consultar_obj(model));
        //}

        public RetornoNotaFiscal ReceberNotaFiscal(Consulta notaFiscal)
        {
            DataBase.SystemEnvironment = notaFiscal.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(notaFiscal.UsuarioId, notaFiscal.IdentificadorNota, Acao.Retorno);

            var prestadorAcesso = ConsultarPrestadorServico(notaFiscal.UsuarioId, notaFiscal.CnpjPrestador, Acao.Retorno, nfe);

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "/" + notaFiscal.IdentificadorNota, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(notaFiscal.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Retorno, "Ocorreu um erro receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro receber a Nota Fiscal (" + notaFiscal.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return CadastrarNotaFiscalRecebida(notaFiscal, nfse);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(notaFiscal.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, Acao.Retorno, "Ocorreu um erro cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + notaFiscal.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscal CadastrarNotaFiscalRecebida(Consulta notaFiscalRecebida, string retorno)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscal>(retorno);

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

                retornoErro.ErroId = new Tabelas.NfeWsErroController().Cadastrar(retornoErro);

                retornoErro = new Tabelas.NfeWsErroController().Selecionar(retornoErro.ErroId).FirstOrDefault();

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

            retornoConsulta.NotaFiscalId = new Tabelas.NfeRetornoController().Cadastrar(notaFiscal);

            return retornoConsulta;
        }

        public string CancelarNotaFiscal(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(model.UsuarioId, model.IdentificadorNota, Acao.Cancelamento);

            var prestadorAcesso = ConsultarPrestadorServico(model.UsuarioId, model.CnpjPrestador, Acao.Cancelamento, nfe);

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
                CadastrarErroGenerico(model.UsuarioId, nfe.IdentificadorNota.Value, OrigemErro.WebService, Acao.Cancelamento, "Ocorreu um erro cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private void CadastrarErroGenerico(int usuarioId, int identificadorNota, OrigemErro origemErro, Acao acao, string mensagemErro)
        {
            var erro = new NfeWsErroModel
            {
                IdentificadorNota = identificadorNota,
                UsuarioId = usuarioId,
                Acao = (char)acao,
                OrigemErro = (char)origemErro,
                MensagemErro = mensagemErro
            };

            try
            {
                new Tabelas.NfeWsErroController().Cadastrar(erro);
            }
            catch { }
        }

        private Nfe ConsultarNotaFiscal(int usuarioId, int identificadorNota, Acao acao)
        {
            List<Nfe> nfe;

            try
            {
                if ((nfe = new Tabelas.Nfe().Consultar(identificadorNota)) == null)
                {
                    CadastrarErroGenerico(usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Nota Fiscal não encontrada no cadastro do Depósito Público");

                    throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público (" + identificadorNota + ")");
                }
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            return nfe.FirstOrDefault();
        }

        private PrestadorServico ConsultarPrestadorServico(int usuarioId, string cnpj, Acao acao, Nfe nfe, CapaAutorizacaoNfse capaAutorizacaoNfse = null)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            cnpj = cnpj.Replace("/", "").Replace(".", "").Replace("-", "");

            var prestadorAcesso = new PrestadorServico();

            try
            {
                prestadorAcesso.server = new Tabelas.Server().GetRemoteServer();
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar os dados do Servidor: " + ex);

                throw new Exception("Ocorreu um erro ao consultar os dados do Servidor: " + ex);
            }

            try
            {
                using (var dtPrestador = new Tabelas.Prestador().Consultar(cnpj, capaAutorizacaoNfse))
                {
                    if (dtPrestador == null)
                    {
                        CadastrarErroGenerico(usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Prestador de Serviços não configurado. CNPJ: " + cnpj);

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
                CadastrarErroGenerico(usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);

                throw new Exception("Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);
            }

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                CadastrarErroGenerico(usuarioId, nfe.IdentificadorNota.Value, OrigemErro.MobLink, acao, "Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);

                throw new Exception("Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);
            }

            return prestadorAcesso;
        }

        /// <summary>
        /// Método obsoleto que seleciona os registros necessários para a Emissão da Nota Fiscal
        /// </summary>
        /// <param name="grvId"></param>
        /// <param name="isDev"></param>
        /// <returns></returns>
        //public string GerarNota(int grvId, bool isDev)
        //{
        //    DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

        //    Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

        //    var nfe = new List<Nfe>();

        //    try
        //    {
        //        nfe = new Tabelas.Nfe().ConsultarPorGrv(grvId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal: " + ex.Message);
        //    }

        //    if (nfe == null)
        //    {
        //        throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público.");
        //    }

        //    var dataTable = new Tabelas.Grv().Consultar(grvId);

        //    if (dataTable == null)
        //    {


        //        return "Sem dados para geração da nota!";
        //    }

        //    CapaAutorizacaoNfse capaAutorizacaoNfse = new CapaAutorizacaoNfse();

        //    foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
        //    {
        //        if (row["nota_fiscal_nome"].ToString().Equals(""))
        //            return "Dados insuficientes para geração da nota";

        //        capaAutorizacaoNfse.Homologacao = isDev;

        //        capaAutorizacaoNfse.Autorizar = new Autorizar
        //        {
        //            data_emissao = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"),
        //            natureza_operacao = "1",
        //            optante_simples_nacional = "false",

        //            prestador = new Prestador
        //            {
        //                cnpj = row["cnpj"].ToString(),
        //                inscricao_municipal = row["inscricao_municipal"].ToString(),
        //                codigo_municipio = row["codigo_municipio_ibge"].ToString()
        //            },

        //            tomador = new Tomador
        //            {
        //                razao_social = row["nota_fiscal_nome"].ToString(),
        //                email = row["nota_fiscal_email"].ToString()
        //            }
        //        };

        //        if (row["nota_fiscal_cpf"].ToString().Length > 11)
        //            capaAutorizacaoNfse.Autorizar.tomador.cnpj = row["nota_fiscal_cpf"].ToString();
        //        else
        //            capaAutorizacaoNfse.Autorizar.tomador.cpf = row["nota_fiscal_cpf"].ToString();

        //        capaAutorizacaoNfse.Autorizar.tomador.endereco = new Endereco
        //        {
        //            logradouro = row["nota_fiscal_endereco"].ToString(),
        //            numero = row["nota_fiscal_numero"].ToString(),
        //            complemento = row["nota_fiscal_complemento"].ToString(),
        //            bairro = row["nota_fiscal_bairro"].ToString(),
        //            codigo_municipio = row["codigo_municipio_ibge"].ToString(),
        //            uf = row["nota_fiscal_uf"].ToString(),
        //            cep = row["nota_fiscal_cep"].ToString()
        //        };

        //        capaAutorizacaoNfse.Autorizar.servico = new Servico
        //        {
        //            aliquota = "3.00",
        //            discriminacao = "Nota fiscal referente a serviços prestados",
        //            iss_retido = "false",
        //            valor_iss = "0",
        //            item_lista_servico = "0801",
        //            codigo_tributario_municipio = "080101",
        //            valor_servicos = row["valor_pagamento"].ToString().Replace(',', '.')
        //        };

        //        capaAutorizacaoNfse.CodigoRetorno = Convert.ToInt32(row["id_faturamento"]);
        //    }

        //    return AutorizarNfse(capaAutorizacaoNfse);
        //}
    }
}