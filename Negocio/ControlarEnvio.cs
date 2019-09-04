using Negocio.Util;
using NFSE.Domain.Entities;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace Negocio
{
    public class ControlarEnvio
    {
        public string AutorizarNfse(CapaAutorizacaoNfse model)
        {
            string str = model.identificador_nota.ToString();

            PrestadorAcesso prestadorAcesso = VerificarPrestador(model.autorizar.prestador.cnpj.Replace("/", "").Replace(".", "").Replace("-", ""), model.homologacao, model);

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                return "Prestador não configurado";
            }

            string uri = prestadorAcesso.server + "v2/nfse?ref=" + str;

            model.autorizar.servico.codigo_tributario_municipio = !prestadorAcesso.codigo_tributario_municipio.Equals("") ? prestadorAcesso.codigo_tributario_municipio : (string)null;
            model.autorizar.servico.item_lista_servico = prestadorAcesso.item_lista_servico;
            model.autorizar.servico.codigo_cnae = prestadorAcesso.codigo_cnae;

            var tools = new Tools();

            string json = tools.ObjToJSON((object)model.autorizar);

            string resposta;

            try
            {
                resposta = tools.PostNfse(uri, json, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro ao executar o WebService:\n\n" + ex.Message;
            }

            try
            {
                new Tabelas.AutorizacaoNotaFiscal().Cadastrar(prestadorAcesso, model, resposta);
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro ao cadastrar a Nota:\n\n" + ex.Message;
            }

            return resposta;
        }


        public string Consultar(Consultar obj)
        {
            PrestadorAcesso prestadorAcesso = VerificarPrestador(obj.cnpj_prestador.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao);

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                return "Prestador não configurado";
            }

            string nfse = new Tools().GetNfse(prestadorAcesso.server + "v2/nfse/" + obj.referencia, prestadorAcesso.prestador_chave);

            string str = "";

            try
            {
                str = InserirConsulta(prestadorAcesso, obj, nfse);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal:\n\n" + ex.Message);
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return nfse;
            }

            return str;
        }

        public RetornoConsulta Consultar_obj(Consultar obj)
        {
            var prestadorAcesso = new PrestadorAcesso();

            try
            {
                prestadorAcesso = VerificarPrestador(obj.cnpj_prestador.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                throw new Exception("Prestador sem chave configurada (token)");
            }

            string nfse;

            try
            {
                nfse = new Tools().GetNfse(prestadorAcesso.server + "v2/nfse/" + obj.referencia, prestadorAcesso.prestador_chave);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                return InserirConsulta_obj(prestadorAcesso, obj, nfse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string InserirConsulta(PrestadorAcesso prestadorAcesso, Consultar consultar, string retorno)
        {
            return new JavaScriptSerializer().Serialize((object)InserirConsulta_obj(prestadorAcesso, consultar, retorno));
        }

        private RetornoConsulta InserirConsulta_obj(PrestadorAcesso prestadorAcesso, Consultar consultar, string retorno)
        {
            DataBase.SystemEnvironment = consultar.homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            var retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoConsulta>(retorno);

            if (retornoConsulta.url == null)
            {
                retorno = retorno.Replace("\"codigo\"", "\"codigoerro\"");
                retorno = retorno.Replace("\"mensagem\"", "\"mensagemerro\"");

                var retornoErro = new JavaScriptSerializer()
                {
                    MaxJsonLength = int.MaxValue
                }.Deserialize<RetornoErro>(retorno);

                retornoErro.AutorizacaoNotaFiscalId = int.Parse(consultar.referencia);
                retornoErro.UsuarioId = consultar.id_usuario;
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
                AutorizacaoNotaFiscalId = int.Parse(consultar.referencia),
                UsuarioId = consultar.id_usuario,
                FlagAmbiente = consultar.homologacao ? "1" : "0",
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

            try
            {
                retornoConsulta.NotaFiscalId = new Tabelas.NotaFiscal().Cadastrar(notaFiscal);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal:\n\n" + ex.Message);
            }

            return retornoConsulta;
        }

        public string Cancelar(Cancelar cancelar)
        {
            DataBase.SystemEnvironment = cancelar.homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            string str1 = cancelar.referencia.ToString();
            string server = GetRemoteServer();
            string token;

            if (cancelar.homologacao)
            {
                token = "2D6xPXxoXRyIuTyUjS6HbiLao7Xr50Mb";
            }
            else
            {
                token = "1Zrf7fOmWSdLwtOZZVGcFJRhl9SFps1x";
            }

            string uri = server + "v2/nfse/" + str1;

            var tools = new Tools();

            string json = tools.ObjToJSON((object)new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    cancelar.justificativa
                }
            });

            return tools.CancelarNfse(uri, json, token);
        }

        public string GerarNota(int id_grv, bool isDev)
        {
            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            var dataTable = new Tabelas.Grv().Consultar(id_grv);

            if (dataTable == null)
                return "Sem dados para geração da nota!";
            CapaAutorizacaoNfse capaAutorizacaoNfse = new CapaAutorizacaoNfse();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
            {
                if (row["nota_fiscal_nome"].ToString().Equals(""))
                    return "Dados insuficientes para geração da nota";

                capaAutorizacaoNfse.homologacao = isDev;

                capaAutorizacaoNfse.autorizar = new Autorizar
                {
                    data_emissao = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss"),
                    natureza_operacao = "1",
                    optante_simples_nacional = "false",

                    prestador = new Prestador
                    {
                        cnpj = row["cnpj"].ToString(),
                        inscricao_municipal = row["inscricao_municipal"].ToString(),
                        codigo_municipio = row["codigo_municipio_ibge"].ToString()
                    },

                    tomador = new Tomador
                    {
                        razao_social = row["nota_fiscal_nome"].ToString(),
                        email = row["nota_fiscal_email"].ToString()
                    }
                };

                if (row["nota_fiscal_cpf"].ToString().Length > 11)
                    capaAutorizacaoNfse.autorizar.tomador.cnpj = row["nota_fiscal_cpf"].ToString();
                else
                    capaAutorizacaoNfse.autorizar.tomador.cpf = row["nota_fiscal_cpf"].ToString();

                capaAutorizacaoNfse.autorizar.tomador.endereco = new Endereco
                {
                    logradouro = row["nota_fiscal_endereco"].ToString(),
                    numero = row["nota_fiscal_numero"].ToString(),
                    complemento = row["nota_fiscal_complemento"].ToString(),
                    bairro = row["nota_fiscal_bairro"].ToString(),
                    codigo_municipio = row["codigo_municipio_ibge"].ToString(),
                    uf = row["nota_fiscal_uf"].ToString(),
                    cep = row["nota_fiscal_cep"].ToString()
                };

                capaAutorizacaoNfse.autorizar.servico = new Servico
                {
                    aliquota = "3.00",
                    discriminacao = "Nota fiscal referente a serviços prestados",
                    iss_retido = "false",
                    valor_iss = "0",
                    item_lista_servico = "0801",
                    codigo_tributario_municipio = "080101",
                    valor_servicos = row["valor_pagamento"].ToString().Replace(',', '.')
                };

                capaAutorizacaoNfse.identificador_nota = Convert.ToInt32(row["id_faturamento"]);
            }

            return AutorizarNfse(capaAutorizacaoNfse);
        }

        public PrestadorAcesso VerificarPrestador(string cnpj, bool isDev, CapaAutorizacaoNfse capaAutorizacaoNfse)
        {
            var prestadorAcesso = new PrestadorAcesso();

            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            prestadorAcesso.server = GetRemoteServer();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            var dtPrestador = new Tabelas.Prestador().Consultar(cnpj, capaAutorizacaoNfse);

            if (dtPrestador == null)
            {
                return prestadorAcesso;
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

            DataBase.DisconnectDataBase();

            return prestadorAcesso;
        }

        public PrestadorAcesso VerificarPrestador(string cnpj, bool isDev)
        {
            DataBase.SystemEnvironment = isDev ? SystemEnvironment.Development : SystemEnvironment.Production;

            var prestadorAcesso = new PrestadorAcesso();

            prestadorAcesso.server = GetRemoteServer();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            var dtPrestador = new Tabelas.Prestador().Consultar(cnpj);

            if (dtPrestador == null)
            {
                return prestadorAcesso;
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

            DataBase.DisconnectDataBase();

            return prestadorAcesso;
        }

        public string GetRemoteServer()
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT Server");
            SQL.AppendLine("  FROM " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_configuracoes");

            var dtConfiguracoes = DataBase.Select(SQL);

            return dtConfiguracoes.Rows[0]["Server"].ToString();
        }
    }
}