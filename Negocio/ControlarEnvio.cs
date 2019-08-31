using Negocio.Modelo;
using Negocio.Util;
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

            PrestadorAcesso prestadorAcesso = this.VerificarPrestador(model.autorizar.prestador.cnpj.Replace("/", "").Replace(".", "").Replace("-", ""), model.homologacao, model);

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
                InserirNota(prestadorAcesso, model, resposta);
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro ao cadastrar a Nota:\n\n" + ex.Message;
            }

            return resposta;
        }

        private void InserirNota(PrestadorAcesso _param, CapaAutorizacaoNfse obj, string resposta)
        {
            ConnectionFactory.Executar(string.Format("INSERT INTO {0}.dbo.tb_nfse_autorizacao_dp_nf(referencia_externa,resposta_envio,id_nfse_prestador, id_usuario, flag_ambiente, data_emissao, natureza_operacao, optante_simples_nacional, tomador_cpf_cnpj, tomador_cnpj, tomador_nome_razao_social, tomador_telefone, tomador_email, tomador_endereco_logradouro, tomador_endereco_numero, tomador_endereco_complemento, tomador_endereco_bairro, tomador_endereco_codigo_municipio, tomador_endereco_uf, tomador_endereco_cep, servico_aliquota, servico_discriminacao, servico_iss_retido, servico_valor_iss, servico_codigo_cnae, servico_item_lista_servico, servico_valor_servicos) \r\n                    VALUES      (", (object)_param._base).ToString() + "'" + (object)obj.identificador_nota + "'," + "'" + resposta + "'," + _param.id_nfse_prestador + "," + (object)obj.id_usuario + "," + "'" + (object)Convert.ToInt32(obj.homologacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.data_emissao) ? "" : obj.autorizar.data_emissao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.natureza_operacao) ? "" : obj.autorizar.natureza_operacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.optante_simples_nacional) ? "" : Convert.ToInt32(Convert.ToBoolean(obj.autorizar.optante_simples_nacional)).ToString()) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.cpf) ? "" : obj.autorizar.tomador.cpf) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.cnpj) ? "" : obj.autorizar.tomador.cnpj) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.razao_social) ? "" : obj.autorizar.tomador.razao_social) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.telefone) ? "" : obj.autorizar.tomador.telefone) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.email) ? "" : obj.autorizar.tomador.email) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.logradouro) ? "" : obj.autorizar.tomador.endereco.logradouro) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.numero) ? "" : obj.autorizar.tomador.endereco.numero) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.complemento) ? "" : obj.autorizar.tomador.endereco.complemento) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.bairro) ? "" : obj.autorizar.tomador.endereco.bairro) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.codigo_municipio) ? "" : obj.autorizar.tomador.endereco.codigo_municipio) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.uf) ? "" : obj.autorizar.tomador.endereco.uf) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.cep) ? "" : obj.autorizar.tomador.endereco.cep) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.aliquota) ? "" : obj.autorizar.servico.aliquota) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.discriminacao) ? "" : obj.autorizar.servico.discriminacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.iss_retido) ? "" : Convert.ToInt32(Convert.ToBoolean(obj.autorizar.servico.iss_retido)).ToString()) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.valor_iss) ? "" : obj.autorizar.servico.valor_iss) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.codigo_cnae) ? "" : obj.autorizar.servico.codigo_cnae) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.item_lista_servico) ? "" : obj.autorizar.servico.item_lista_servico) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.valor_servicos) ? "" : obj.autorizar.servico.valor_servicos) + "')", false);
        }

        public string Consultar(Consultar obj)
        {
            PrestadorAcesso prestadorAcesso = this.VerificarPrestador(obj.cnpj_prestador.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao);

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
                return "Prestador não configurado";

            string nfse = new Tools().GetNfse(prestadorAcesso.server + "v2/nfse/" + obj.referencia, prestadorAcesso.prestador_chave);

            string str = "";

            try
            {
                str = this.InserirConsulta(prestadorAcesso, obj, nfse);
            }
            catch
            {
            }

            if (str.Trim().Equals(""))
                return nfse;

            return str;
        }

        private string InserirConsulta(PrestadorAcesso _param, Consultar obj, string retorno)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionFactory.connectionString);
            SqlCommand sqlCommand = new SqlCommand();
            sqlConnection.Open();
            Tools tools = new Tools();

            if (sqlConnection.State == ConnectionState.Open)
                sqlCommand.Connection = sqlConnection;
            RetornoConsulta retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoConsulta>(retorno);

            if (retornoConsulta.url.Equals(""))
            {
                string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal) VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal)", (object)_param._base);
                sqlCommand.CommandText = str;
                sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object)obj.referencia;
                sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object)obj.id_usuario;
                sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object)Convert.ToInt32(obj.homologacao);
                sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object)retornoConsulta.status;
                sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object)retornoConsulta.numero;
                sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object)retornoConsulta.codigo_verificacao;
                sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object)retornoConsulta.data_emissao;
                sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object)retornoConsulta.url;
                sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object)retornoConsulta.caminho_xml_nota_fiscal;
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                sqlCommand.Parameters.Clear();
            }
            else
            {
                byte[] numArray = (byte[])null;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");
                    tools.ObterImagemEndereco(retornoConsulta.url).Save((Stream)memoryStream, ImageFormat.Jpeg);
                    numArray = memoryStream.ToArray();
                }

                string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal, imagem_nota_fiscal)\r\n                                            VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal, @imagem_nota_fiscal)", (object)_param._base);

                sqlCommand.CommandText = str;
                sqlCommand.Parameters.Add("@imagem_nota_fiscal", SqlDbType.VarBinary, numArray.Length).Value = (object)numArray;
                sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object)obj.referencia;
                sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object)obj.id_usuario;
                sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object)Convert.ToInt32(obj.homologacao);
                sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object)retornoConsulta.status;
                sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object)retornoConsulta.numero;
                sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object)retornoConsulta.codigo_verificacao;
                sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object)retornoConsulta.data_emissao;
                sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object)retornoConsulta.url;
                sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object)retornoConsulta.caminho_xml_nota_fiscal;

                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                sqlCommand.Parameters.Clear();

                retornoConsulta.ImagemNotaFiscal = numArray;
            }

            sqlConnection.Close();

            return new JavaScriptSerializer().Serialize((object)retornoConsulta);
        }

        private RetornoConsulta InserirConsulta_obj(PrestadorAcesso _param, Consultar obj, string retorno)
        {
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

                retornoErro.AutorizacaoNotaFiscalId = int.Parse(obj.referencia);
                retornoErro.UsuarioId = obj.id_usuario;
                retornoErro.CodigoErro = retornoErro.CodigoErro.Trim().ToUpper();
                retornoErro.MensagemErro = retornoErro.MensagemErro.Trim();

                retornoConsulta.NotaFiscalErroId = CadastrarRetornoErroNotaFiscal(_param._base, retornoErro);

                retornoConsulta.AutorizacaoNotaFiscalId = retornoErro.AutorizacaoNotaFiscalId;
                retornoConsulta.UsuarioId = retornoErro.UsuarioId;
                retornoConsulta.CodigoErro = retornoErro.CodigoErro;
                retornoConsulta.MensagemErro = retornoErro.MensagemErro;

                return retornoConsulta;
            }

            var notaFiscal = new NotaFiscal
            {
                AutorizacaoNotaFiscalId = int.Parse(obj.referencia),
                UsuarioId = obj.id_usuario,
                FlagAmbiente = obj.homologacao ? "1" : "0",
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

                    new Tools().ObterImagemEndereco(notaFiscal.UrlNotaFiscal).Save((Stream)memoryStream, ImageFormat.Jpeg);

                    notaFiscal.ImagemNotaFiscal = memoryStream.ToArray();
                }

                retornoConsulta.ImagemNotaFiscal = notaFiscal.ImagemNotaFiscal;
            }

            try
            {
                retornoConsulta.NotaFiscalId = CadastrarRetornoNotaFiscal(_param._base, notaFiscal);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal:\n\n" + ex.Message);
            }

            return retornoConsulta;
        }

        public string Cancelar(Cancelar obj)
        {
            string str1 = obj.referencia.ToString();
            string str2;
            string token;

            if (obj.homologacao)
            {
                str2 = "http://homologacao.acrasnfe.acras.com.br/";
                token = "2D6xPXxoXRyIuTyUjS6HbiLao7Xr50Mb";
            }
            else
            {
                str2 = "https://api.focusnfe.com.br/";
                token = "1Zrf7fOmWSdLwtOZZVGcFJRhl9SFps1x";
            }

            string uri = str2 + "v2/nfse/" + str1;
            Tools tools = new Tools();
            string json = tools.ObjToJSON((object)new Dictionary<string, string>()
            {
                {
                    "justificativa",
                    obj.justificativa
                }
            });

            return tools.CancelarNfse(uri, json, token);
        }

        public string GerarNota(int id_grv, bool _isdev)
        {
            ConnectionFactory.connectionString = !_isdev ? "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=ws_patio;Password=Studio55#" : "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            DataTable dataTable = ConnectionFactory.Consultar(string.Format("SELECT TOP 1\r\n                 tb_dep_Grv.id_grv\r\n                , tb_dep_Grv.id_status_operacao\r\n                , tb_dep_grv.id_cliente\r\n                , tb_dep_Grv.numero_formulario_grv\r\n                , db_global..tb_glo_emp_empresas.nome\r\n                , db_global..tb_glo_emp_empresas.inscricao_municipal\r\n                , db_global..tb_glo_emp_empresas.cnpj\r\n                , tb_dep_clientes.nome\r\n                , tb_glo_loc_municipios.codigo_municipio_ibge\r\n                , tb_dep_atendimento.nota_fiscal_nome\r\n                , tb_dep_atendimento.nota_fiscal_cpf\r\n                , tb_dep_atendimento.nota_fiscal_endereco\r\n                , tb_dep_atendimento.nota_fiscal_numero\r\n                , tb_dep_atendimento.nota_fiscal_complemento\r\n                , tb_dep_atendimento.nota_fiscal_bairro\r\n                , tb_dep_atendimento.nota_fiscal_municipio\r\n                , tb_dep_atendimento.nota_fiscal_uf\r\n                , tb_dep_atendimento.nota_fiscal_cep\r\n                , tb_dep_atendimento.nota_fiscal_ddd\r\n                , tb_dep_atendimento.nota_fiscal_telefone\r\n                , tb_dep_atendimento.nota_fiscal_email\r\n                , tb_dep_faturamento.id_faturamento\r\n                , tb_dep_faturamento.data_pagamento\r\n                , tb_dep_faturamento.valor_pagamento\r\n                FROM tb_dep_Grv\r\n                JOIN tb_dep_clientes ON tb_dep_grv.id_cliente = tb_dep_clientes.id_cliente\r\n                JOIN  db_global..tb_glo_emp_empresas ON db_global..tb_glo_emp_empresas.id_empresa = tb_dep_clientes.id_empresa \r\n                JOIN  db_global..tb_glo_loc_cep ON db_global..tb_glo_loc_cep.id_cep = tb_dep_clientes.id_cep\r\n                JOIN  db_global..tb_glo_loc_municipios ON db_global..tb_glo_loc_municipios.id_municipio = db_global..tb_glo_loc_cep.id_municipio\r\n                JOIN tb_dep_atendimento ON tb_dep_atendimento.id_grv = tb_dep_grv.id_grv\r\n                JOIN tb_dep_faturamento ON tb_dep_faturamento.id_atendimento = tb_dep_atendimento.id_atendimento\r\n                WHERE tb_dep_Grv.id_grv IN ({0})\r\n                AND   tb_dep_Grv.id_status_operacao = 'E'", (object)id_grv).ToString(), false);

            if (dataTable == null)
                return "Sem dados para geração da nota!";
            CapaAutorizacaoNfse capaAutorizacaoNfse = new CapaAutorizacaoNfse();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
            {
                if (row["nota_fiscal_nome"].ToString().Equals(""))
                    return "Dados insuficientes para geração da nota";

                capaAutorizacaoNfse.homologacao = _isdev;

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

            return this.AutorizarNfse(capaAutorizacaoNfse);
        }

        public PrestadorAcesso VerificarPrestador(string cnpj, bool _isdev, CapaAutorizacaoNfse obj)
        {
            var prestadorAcesso = new PrestadorAcesso();

            if (_isdev)
            {
                prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
                prestadorAcesso._base = "db_NfseDev";
                // prestadorAcesso.server = "http://homologacao.acrasnfe.acras.com.br/";
            }
            else
            {
                prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
                prestadorAcesso._base = "db_Nfse";
                // prestadorAcesso.server = "https://api.focusnfe.com.br/";
            }

            ConnectionFactory.connectionString = prestadorAcesso.connection;

            prestadorAcesso.server = GetRemoteServer(_isdev);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            var dtPrestador = ConnectionFactory.Consultar(string.Format(@"SELECT a.id_nfse_prestador,
                                           a.prestador_cnpj,
                                           a.prestador_nome,
                                           a.prestador_inscricao_municipal,
                                           a.prestador_codigo_municipio_ibge,
                                           a.prestador_chave,
                                           a.prestador_data_cadastro,
                                           b.item_lista_servico,         
                                           b.codigo_tributario_municipio,
                                           b.codigo_cnae  
                                    FROM   {1}.dbo.tb_nfse_prestador a left join  {1}.dbo.tb_nfse_parametro_municipio b  on
                                           b.codigo_ibge = a.prestador_codigo_municipio_ibge
                                           WHERE  prestador_cnpj='{0}'
						                          and b.item_lista_servico = '{2}'
                                                  and a.prestador_codigo_municipio_ibge = '{3}'", (object)cnpj, (object)prestadorAcesso._base, obj.autorizar.servico.item_lista_servico, obj.autorizar.prestador.codigo_municipio).ToString(), false);

            if (dtPrestador == null)
                return prestadorAcesso;

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

            ConnectionFactory.Desconectar();

            return prestadorAcesso;
        }

        public PrestadorAcesso VerificarPrestador(string cnpj, bool _isdev)
        {
            var prestadorAcesso = new PrestadorAcesso();

            if (_isdev)
            {
                prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
                prestadorAcesso._base = "db_NfseDev";
                // prestadorAcesso.server = "http://homologacao.acrasnfe.acras.com.br/";
            }
            else
            {
                prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
                prestadorAcesso._base = "db_Nfse";
                // prestadorAcesso.server = "https://api.focusnfe.com.br/";
            }

            ConnectionFactory.connectionString = prestadorAcesso.connection;

            prestadorAcesso.server = GetRemoteServer(_isdev);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            DataTable dataTable = ConnectionFactory.Consultar(string.Format(@"SELECT a.id_nfse_prestador,
                                           a.prestador_cnpj,
                                           a.prestador_nome,
                                           a.prestador_inscricao_municipal,
                                           a.prestador_codigo_municipio_ibge,
                                           a.prestador_chave,
                                           a.prestador_data_cadastro,
                                           b.item_lista_servico,         
                                           b.codigo_tributario_municipio,
                                           b.codigo_cnae  
                                    FROM   {1}.dbo.tb_nfse_prestador a left join  {1}.dbo.tb_nfse_parametro_municipio b  on
                                           b.codigo_ibge = a.prestador_codigo_municipio_ibge
                                           WHERE  prestador_cnpj='{0}'", (object)cnpj, (object)prestadorAcesso._base).ToString(), false);

            if (dataTable == null)
            {
                return prestadorAcesso;
            }

            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
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

            ConnectionFactory.Desconectar();

            return prestadorAcesso;
        }

        public RetornoConsulta Consultar_obj(Consultar obj)
        {
            PrestadorAcesso prestadorAcesso = VerificarPrestador(obj.cnpj_prestador.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao);

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                return (RetornoConsulta)null;
            }

            string nfse = new Tools().GetNfse(prestadorAcesso.server + "v2/nfse/" + obj.referencia, prestadorAcesso.prestador_chave);

            try
            {
                return InserirConsulta_obj(prestadorAcesso, obj, nfse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetRemoteServer(bool isDev)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT Server");
            SQL.AppendLine("  FROM " + (isDev ? "db_NfseDev" : "db_Nfse") + ".dbo.tb_nfse_configuracoes");

            var dtConfiguracoes = ConnectionFactory.Consultar(SQL.ToString());

            return dtConfiguracoes.Rows[0]["Server"].ToString();
        }

        private int CadastrarRetornoNotaFiscal(string dataBase, NotaFiscal model)
        {
            using (var sqlConnection = new SqlConnection(ConnectionFactory.connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlCommand.Connection = sqlConnection;
                    }

                    var SQL = new StringBuilder();

                    SQL.AppendLine("INSERT INTO " + dataBase + ".dbo.tb_nfse_consultar_dp_nf");

                    SQL.AppendLine("    (id_autorizacao_nf");
                    SQL.AppendLine("    ,id_usuario");
                    SQL.AppendLine("    ,flag_ambiente");
                    SQL.AppendLine("    ,status_nf");
                    SQL.AppendLine("    ,numero_nf");
                    SQL.AppendLine("    ,codigo_verificacao");
                    SQL.AppendLine("    ,data_emissao");
                    SQL.AppendLine("    ,url_nota_fiscal");

                    if (model.ImagemNotaFiscal != null)
                    {
                        SQL.AppendLine("    ,imagem_nota_fiscal");
                    }

                    SQL.AppendLine("    ,caminho_xml_nota_fiscal)");

                    SQL.AppendLine("VALUES");

                    SQL.AppendLine("        (@id_autorizacao_nf");
                    SQL.AppendLine("        ,@id_usuario");
                    SQL.AppendLine("        ,@flag_ambiente");
                    SQL.AppendLine("        ,@status_nf");
                    SQL.AppendLine("        ,@numero_nf");
                    SQL.AppendLine("        ,@codigo_verificacao");
                    SQL.AppendLine("        ,@data_emissao");
                    SQL.AppendLine("        ,@url_nota_fiscal");

                    if (model.ImagemNotaFiscal != null)
                    {
                        SQL.AppendLine("        ,@imagem_nota_fiscal");
                    }

                    SQL.AppendLine("        ,@caminho_xml_nota_fiscal)");

                    sqlCommand.CommandText = SQL.ToString();

                    sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = model.AutorizacaoNotaFiscalId;
                    sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = model.UsuarioId;
                    sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.VarChar).Value = model.FlagAmbiente;
                    sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = model.StatusNotaFiscal;
                    sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = model.NumeroNotaFiscal;
                    sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = model.CodigoVerificacao;
                    sqlCommand.Parameters.Add("@data_emissao", SqlDbType.DateTime).Value = model.DataEmissao;
                    sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = model.UrlNotaFiscal;
                    sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = model.CaminhoNotaFiscal;

                    if (model.ImagemNotaFiscal != null)
                    {
                        sqlCommand.Parameters.Add("@imagem_nota_fiscal", SqlDbType.VarBinary, model.ImagemNotaFiscal.Length).Value = model.ImagemNotaFiscal;
                    }

                    sqlCommand.CommandText = SQL.ToString() + "; SELECT CAST(scope_identity() AS INT)";

                    return (Int32)sqlCommand.ExecuteScalar();
                }
            }
        }

        private int CadastrarRetornoErroNotaFiscal(string dataBase, RetornoErro model)
        {
            using (var sqlConnection = new SqlConnection(ConnectionFactory.connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand())
                {
                    if (sqlConnection.State == ConnectionState.Open)
                    {
                        sqlCommand.Connection = sqlConnection;
                    }

                    var SQL = new StringBuilder();

                    SQL.AppendLine("INSERT INTO " + dataBase + ".dbo.tb_nfse_consultar_dp_nf_erro");

                    SQL.AppendLine("    (AutorizacaoNotaFiscalId");
                    SQL.AppendLine("    ,UsuarioId");
                    SQL.AppendLine("    ,CodigoErro");
                    SQL.AppendLine("    ,MensagemErro)");

                    SQL.AppendLine("VALUES");

                    SQL.AppendLine("    (@AutorizacaoNotaFiscalId");
                    SQL.AppendLine("    ,@UsuarioId");
                    SQL.AppendLine("    ,@CodigoErro");
                    SQL.AppendLine("    ,@MensagemErro)");

                    sqlCommand.Parameters.Add("@AutorizacaoNotaFiscalId", SqlDbType.Int).Value = model.AutorizacaoNotaFiscalId;
                    sqlCommand.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = model.UsuarioId;
                    sqlCommand.Parameters.Add("@CodigoErro", SqlDbType.VarChar).Value = model.CodigoErro;
                    sqlCommand.Parameters.Add("@MensagemErro", SqlDbType.VarChar).Value = model.MensagemErro;

                    sqlCommand.CommandText = SQL.ToString() + "; SELECT CAST(scope_identity() AS INT)";

                    return (Int32)sqlCommand.ExecuteScalar();
                }
            }
        }
    }
}