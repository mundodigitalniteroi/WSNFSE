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

            var nfe = ConsultarNotaFiscal(capaAutorizacaoNfse.UsuarioId, capaAutorizacaoNfse.IdentificadorNota, 0);

            var prestadorAcesso = ConsultarPrestadorServico(capaAutorizacaoNfse.UsuarioId, capaAutorizacaoNfse.Autorizacao.prestador.cnpj, nfe, capaAutorizacaoNfse);

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
                CadastrarErroGenerico(capaAutorizacaoNfse.UsuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.WebService, "Ocorreu um erro ao solicitar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao solicitar a Nota Fiscal (" + capaAutorizacaoNfse.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                new Tabelas.AutorizacaoNotaFiscal().Cadastrar(prestadorAcesso, capaAutorizacaoNfse, resposta);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(capaAutorizacaoNfse.UsuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Ocorreu um erro ao cadastrar a solicitação da Nota Fiscal: " + ex.Message);

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

            var nfe = ConsultarNotaFiscal(notaFiscal.UsuarioId, notaFiscal.IdentificadorNota, 0);

            var prestadorAcesso = ConsultarPrestadorServico(notaFiscal.UsuarioId, notaFiscal.CnpjPrestador, nfe);

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "/" + notaFiscal.IdentificadorNota, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(notaFiscal.UsuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.WebService, "Ocorreu um erro receber a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro receber a Nota Fiscal (" + notaFiscal.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return CadastrarNotaFiscalRecebida(notaFiscal, nfse);
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(notaFiscal.UsuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Ocorreu um erro cadastrar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro consultar a Nota Fiscal (" + notaFiscal.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscal CadastrarNotaFiscalRecebida(Consulta notaFiscalRecebida, string retorno)
        {
            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscal>(retorno);

            if (retornoConsulta.url == null)
            {
                retorno = retorno.Replace("\"codigo\"", "\"codigoerro\"");
                retorno = retorno.Replace("\"mensagem\"", "\"mensagemerro\"");

                var retornoErro = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<RetornoErro>(retorno);

                retornoErro.AutorizacaoNotaFiscalId = notaFiscalRecebida.IdentificadorNota;
                retornoErro.UsuarioId = notaFiscalRecebida.UsuarioId;
                retornoErro.CodigoErro = retornoErro.CodigoErro.Trim().ToUpper();
                retornoErro.MensagemErro = retornoErro.MensagemErro.Trim();

                retornoConsulta.NotaFiscalErroId = new Tabelas.NotaFiscal().CadastrarErro(retornoErro);

                retornoConsulta.AutorizacaoNotaFiscalId = retornoErro.AutorizacaoNotaFiscalId;
                retornoConsulta.UsuarioId = retornoErro.UsuarioId;
                retornoConsulta.CodigoErro = retornoErro.CodigoErro;
                retornoConsulta.MensagemErro = retornoErro.MensagemErro;

                return retornoConsulta;
            }

            var notaFiscal = new NotaFiscal
            {
                AutorizacaoNotaFiscalId = notaFiscalRecebida.IdentificadorNota,
                UsuarioId = notaFiscalRecebida.UsuarioId,
                FlagAmbiente = notaFiscalRecebida.Homologacao ? "1" : "0",
                StatusNotaFiscal = retornoConsulta.status,
                NumeroNotaFiscal = retornoConsulta.numero,
                CodigoVerificacao = retornoConsulta.codigo_verificacao,
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

            retornoConsulta.NotaFiscalId = new Tabelas.NotaFiscal().Cadastrar(notaFiscal);

            return retornoConsulta;
        }

        public string CancelarNotaFiscal(Cancelamento model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var nfe = ConsultarNotaFiscal(model.UsuarioId, model.IdentificadorNota, 0);

            var prestadorAcesso = ConsultarPrestadorServico(model.UsuarioId, model.CnpjPrestador, nfe);

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
                CadastrarErroGenerico(model.UsuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.WebService, "Ocorreu um erro cancelar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro cancelar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private void CadastrarErroGenerico(int usuarioId, int? idGrv, int? identificadorNota, OrigemErro origemErro, string mensagemErro)
        {
            var erro = new Erro
            {
                UsuarioId = usuarioId,
                GrvId = idGrv != null ? idGrv : null,
                IdentificadorNota = identificadorNota != null ? identificadorNota : null,
                OrigemErro = origemErro == OrigemErro.MobLink ? 'M' : 'W',
                MensagemErro = mensagemErro.ToUpper().Trim()
            };

            try
            {
                new Tabelas.Erro().Cadastrar(erro);
            }
            catch { }
        }

        private Nfe ConsultarNotaFiscal(int usuarioId, int identificadorNota, int idGrv)
        {
            var nfe = new List<Nfe>();

            try
            {
                if (identificadorNota > 0)
                {
                    nfe = new Tabelas.Nfe().ConsultarPorIdentificadorNota(identificadorNota);
                }
                else if (idGrv > 0)
                {
                    nfe = new Tabelas.Nfe().ConsultarPorGrv(idGrv);
                }
            }
            catch (Exception ex)
            {
                CadastrarErroGenerico(usuarioId, idGrv, identificadorNota, OrigemErro.MobLink, "Ocorreu um erro ao consultar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            if (nfe == null)
            {
                CadastrarErroGenerico(usuarioId, idGrv, identificadorNota, OrigemErro.MobLink, "Nota Fiscal não encontrada no cadastro do Depósito Público");

                throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público (" + identificadorNota + ")");
            }

            return nfe.FirstOrDefault();
        }

        private PrestadorServico ConsultarPrestadorServico(int usuarioId, string cnpj, Nfe nfe, CapaAutorizacaoNfse capaAutorizacaoNfse = null)
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
                CadastrarErroGenerico(usuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Ocorreu um erro ao consultar os dados do Servidor: " + ex);

                throw new Exception("Ocorreu um erro ao consultar os dados do Servidor: " + ex);
            }

            try
            {
                using (var dtPrestador = new Tabelas.Prestador().Consultar(cnpj, capaAutorizacaoNfse))
                {
                    if (dtPrestador == null)
                    {
                        CadastrarErroGenerico(usuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Prestador de Serviços não configurado. CNPJ: " + cnpj);

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
                CadastrarErroGenerico(usuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);

                throw new Exception("Ocorreu um erro consultar o Prestador de Serviços: " + ex.Message);
            }

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                CadastrarErroGenerico(usuarioId, nfe.GrvID, nfe.IdentificadorNota, OrigemErro.MobLink, "Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);

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