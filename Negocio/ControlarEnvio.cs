// Decompiled with JetBrains decompiler
// Type: Negocio.ControlarEnvio
// Assembly: Negocio, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3A36FDDC-74CE-410F-B289-948AB9F9B469
// Assembly location: C:\Users\Bruno Werneck\Desktop\GeradorNF\bin\Negocio.dll

using Negocio.Modelo;
using Negocio.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;

namespace Negocio
{
  public class ControlarEnvio
  {
    public string AutorizarNfse(CapaAutorizacaoNfse obj)
    {
      string str = obj.identificador_nota.ToString();
      PrestadorAcesso prestadorAcesso = this.VerificarPrestador(obj.autorizar.prestador.cnpj.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao, obj);
      if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
        return "Prestador não configurado";
      string uri = prestadorAcesso.server + "v2/nfse?ref=" + str;
      Tools tools = new Tools();
      obj.autorizar.servico.codigo_tributario_municipio = !prestadorAcesso.codigo_tributario_municipio.Equals("") ? prestadorAcesso.codigo_tributario_municipio : (string) null;
      obj.autorizar.servico.item_lista_servico = prestadorAcesso.item_lista_servico;
      obj.autorizar.servico.codigo_cnae = prestadorAcesso.codigo_cnae;
      string json = tools.ObjToJSON((object) obj.autorizar);
      string resposta = tools.PostNfse(uri, json, prestadorAcesso.prestador_chave);
      try
      {
        this.InserirNota(prestadorAcesso, obj, resposta);
      }
      catch
      {
      }
      return resposta;
    }

    private void InserirNota(PrestadorAcesso _param, CapaAutorizacaoNfse obj, string resposta)
    {
      ConnectionFactory.Executar(string.Format("INSERT INTO {0}.dbo.tb_nfse_autorizacao_dp_nf \r\n                    (                      \r\n                     referencia_externa,\r\n                     resposta_envio,\r\n                     id_nfse_prestador, \r\n                     id_usuario, \r\n                     flag_ambiente, \r\n                     data_emissao, \r\n                     natureza_operacao, \r\n                     optante_simples_nacional, \r\n                     tomador_cpf_cnpj, \r\n                     tomador_cnpj, \r\n                     tomador_nome_razao_social, \r\n                     tomador_telefone, \r\n                     tomador_email, \r\n                     tomador_endereco_logradouro, \r\n                     tomador_endereco_numero, \r\n                     tomador_endereco_complemento, \r\n                     tomador_endereco_bairro, \r\n                     tomador_endereco_codigo_municipio, \r\n                     tomador_endereco_uf, \r\n                     tomador_endereco_cep, \r\n                     servico_aliquota, \r\n                     servico_discriminacao, \r\n                     servico_iss_retido, \r\n                     servico_valor_iss, \r\n                     servico_codigo_cnae, \r\n                     servico_item_lista_servico, \r\n                     servico_valor_servicos) \r\n                    VALUES      (", (object) _param._base).ToString() + "'" + (object) obj.identificador_nota + "'," + "'" + resposta + "'," + _param.id_nfse_prestador + "," + (object) obj.id_usuario + "," + "'" + (object) Convert.ToInt32(obj.homologacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.data_emissao) ? "" : obj.autorizar.data_emissao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.natureza_operacao) ? "" : obj.autorizar.natureza_operacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.optante_simples_nacional) ? "" : Convert.ToInt32(Convert.ToBoolean(obj.autorizar.optante_simples_nacional)).ToString()) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.cpf) ? "" : obj.autorizar.tomador.cpf) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.cnpj) ? "" : obj.autorizar.tomador.cnpj) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.razao_social) ? "" : obj.autorizar.tomador.razao_social) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.telefone) ? "" : obj.autorizar.tomador.telefone) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.email) ? "" : obj.autorizar.tomador.email) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.logradouro) ? "" : obj.autorizar.tomador.endereco.logradouro) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.numero) ? "" : obj.autorizar.tomador.endereco.numero) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.complemento) ? "" : obj.autorizar.tomador.endereco.complemento) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.bairro) ? "" : obj.autorizar.tomador.endereco.bairro) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.codigo_municipio) ? "" : obj.autorizar.tomador.endereco.codigo_municipio) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.uf) ? "" : obj.autorizar.tomador.endereco.uf) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.tomador.endereco.cep) ? "" : obj.autorizar.tomador.endereco.cep) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.aliquota) ? "" : obj.autorizar.servico.aliquota) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.discriminacao) ? "" : obj.autorizar.servico.discriminacao) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.iss_retido) ? "" : Convert.ToInt32(Convert.ToBoolean(obj.autorizar.servico.iss_retido)).ToString()) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.valor_iss) ? "" : obj.autorizar.servico.valor_iss) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.codigo_cnae) ? "" : obj.autorizar.servico.codigo_cnae) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.item_lista_servico) ? "" : obj.autorizar.servico.item_lista_servico) + "'," + "'" + (string.IsNullOrEmpty(obj.autorizar.servico.valor_servicos) ? "" : obj.autorizar.servico.valor_servicos) + "')", false);
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
        string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal)\r\n                                            VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal)", (object) _param._base);
        sqlCommand.CommandText = str;
        sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object) obj.referencia;
        sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object) obj.id_usuario;
        sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object) Convert.ToInt32(obj.homologacao);
        sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.status;
        sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.numero;
        sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object) retornoConsulta.codigo_verificacao;
        sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object) retornoConsulta.data_emissao;
        sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.url;
        sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.caminho_xml_nota_fiscal;
        sqlCommand.ExecuteNonQuery();
        sqlCommand.Dispose();
        sqlCommand.Parameters.Clear();
      }
      else
      {
        byte[] numArray = (byte[]) null;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");
          tools.ObterImagemEndereco(retornoConsulta.url).Save((Stream) memoryStream, ImageFormat.Jpeg);
          numArray = memoryStream.ToArray();
        }
        string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal, imagem_nota_fiscal)\r\n                                            VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal, @imagem_nota_fiscal)", (object) _param._base);
        sqlCommand.CommandText = str;
        sqlCommand.Parameters.Add("@imagem_nota_fiscal", SqlDbType.VarBinary, numArray.Length).Value = (object) numArray;
        sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object) obj.referencia;
        sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object) obj.id_usuario;
        sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object) Convert.ToInt32(obj.homologacao);
        sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.status;
        sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.numero;
        sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object) retornoConsulta.codigo_verificacao;
        sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object) retornoConsulta.data_emissao;
        sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.url;
        sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.caminho_xml_nota_fiscal;
        sqlCommand.ExecuteNonQuery();
        sqlCommand.Dispose();
        sqlCommand.Parameters.Clear();
        retornoConsulta.retorno_nota = numArray;
      }
      sqlConnection.Close();
      return new JavaScriptSerializer().Serialize((object) retornoConsulta);
    }

    private RetornoConsulta InserirConsulta_obj(PrestadorAcesso _param, Consultar obj, string retorno)
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
        string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal)\r\n                                            VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal)", (object) _param._base);
        sqlCommand.CommandText = str;
        sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object) obj.referencia;
        sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object) obj.id_usuario;
        sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object) Convert.ToInt32(obj.homologacao);
        sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.status;
        sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.numero;
        sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object) retornoConsulta.codigo_verificacao;
        sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object) retornoConsulta.data_emissao;
        sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.url;
        sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.caminho_xml_nota_fiscal;
        sqlCommand.ExecuteNonQuery();
        sqlCommand.Dispose();
        sqlCommand.Parameters.Clear();
      }
      else
      {
        byte[] numArray = (byte[]) null;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");
          tools.ObterImagemEndereco(retornoConsulta.url).Save((Stream) memoryStream, ImageFormat.Jpeg);
          numArray = memoryStream.ToArray();
        }
        string str = string.Format("INSERT INTO {0}.dbo.tb_nfse_consultar_dp_nf (id_autorizacao_nf, id_usuario, flag_ambiente, status_nf, numero_nf, codigo_verificacao, data_emissao, url_nota_fiscal, caminho_xml_nota_fiscal, imagem_nota_fiscal)\r\n                                            VALUES (@id_autorizacao_nf, @id_usuario, @flag_ambiente, @status_nf, @numero_nf, @codigo_verificacao, @data_emissao, @url_nota_fiscal, @caminho_xml_nota_fiscal, @imagem_nota_fiscal)", (object) _param._base);
        sqlCommand.CommandText = str;
        sqlCommand.Parameters.Add("@imagem_nota_fiscal", SqlDbType.VarBinary, numArray.Length).Value = (object) numArray;
        sqlCommand.Parameters.Add("@id_autorizacao_nf", SqlDbType.Int).Value = (object) obj.referencia;
        sqlCommand.Parameters.Add("@id_usuario", SqlDbType.Int).Value = (object) obj.id_usuario;
        sqlCommand.Parameters.Add("@flag_ambiente", SqlDbType.Int).Value = (object) Convert.ToInt32(obj.homologacao);
        sqlCommand.Parameters.Add("@status_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.status;
        sqlCommand.Parameters.Add("@numero_nf", SqlDbType.VarChar).Value = (object) retornoConsulta.numero;
        sqlCommand.Parameters.Add("@codigo_verificacao", SqlDbType.VarChar).Value = (object) retornoConsulta.codigo_verificacao;
        sqlCommand.Parameters.Add("@data_emissao", SqlDbType.VarChar).Value = (object) retornoConsulta.data_emissao;
        sqlCommand.Parameters.Add("@url_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.url;
        sqlCommand.Parameters.Add("@caminho_xml_nota_fiscal", SqlDbType.VarChar).Value = (object) retornoConsulta.caminho_xml_nota_fiscal;
        sqlCommand.ExecuteNonQuery();
        sqlCommand.Dispose();
        sqlCommand.Parameters.Clear();
        retornoConsulta.retorno_nota = numArray;
      }
      sqlConnection.Close();
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
      string json = tools.ObjToJSON((object) new Dictionary<string, string>()
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
      DataTable dataTable = ConnectionFactory.Consultar(string.Format("SELECT TOP 1\r\n                 tb_dep_Grv.id_grv\r\n                , tb_dep_Grv.id_status_operacao\r\n                , tb_dep_grv.id_cliente\r\n                , tb_dep_Grv.numero_formulario_grv\r\n                , db_global..tb_glo_emp_empresas.nome\r\n                , db_global..tb_glo_emp_empresas.inscricao_municipal\r\n                , db_global..tb_glo_emp_empresas.cnpj\r\n                , tb_dep_clientes.nome\r\n                , tb_glo_loc_municipios.codigo_municipio_ibge\r\n                , tb_dep_atendimento.nota_fiscal_nome\r\n                , tb_dep_atendimento.nota_fiscal_cpf\r\n                , tb_dep_atendimento.nota_fiscal_endereco\r\n                , tb_dep_atendimento.nota_fiscal_numero\r\n                , tb_dep_atendimento.nota_fiscal_complemento\r\n                , tb_dep_atendimento.nota_fiscal_bairro\r\n                , tb_dep_atendimento.nota_fiscal_municipio\r\n                , tb_dep_atendimento.nota_fiscal_uf\r\n                , tb_dep_atendimento.nota_fiscal_cep\r\n                , tb_dep_atendimento.nota_fiscal_ddd\r\n                , tb_dep_atendimento.nota_fiscal_telefone\r\n                , tb_dep_atendimento.nota_fiscal_email\r\n                , tb_dep_faturamento.id_faturamento\r\n                , tb_dep_faturamento.data_pagamento\r\n                , tb_dep_faturamento.valor_pagamento\r\n                FROM tb_dep_Grv\r\n                JOIN tb_dep_clientes ON tb_dep_grv.id_cliente = tb_dep_clientes.id_cliente\r\n                JOIN  db_global..tb_glo_emp_empresas ON db_global..tb_glo_emp_empresas.id_empresa = tb_dep_clientes.id_empresa \r\n                JOIN  db_global..tb_glo_loc_cep ON db_global..tb_glo_loc_cep.id_cep = tb_dep_clientes.id_cep\r\n                JOIN  db_global..tb_glo_loc_municipios ON db_global..tb_glo_loc_municipios.id_municipio = db_global..tb_glo_loc_cep.id_municipio\r\n                JOIN tb_dep_atendimento ON tb_dep_atendimento.id_grv = tb_dep_grv.id_grv\r\n                JOIN tb_dep_faturamento ON tb_dep_faturamento.id_atendimento = tb_dep_atendimento.id_atendimento\r\n                WHERE tb_dep_Grv.id_grv IN ({0})\r\n                AND   tb_dep_Grv.id_status_operacao = 'E'", (object) id_grv).ToString(), false);
      if (dataTable == null)
        return "Sem dados para geração da nota!";
      CapaAutorizacaoNfse capaAutorizacaoNfse = new CapaAutorizacaoNfse();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
      {
        if (row["nota_fiscal_nome"].ToString().Equals(""))
          return "Dados insuficientes para geração da nota";
        capaAutorizacaoNfse.homologacao = _isdev;
        capaAutorizacaoNfse.autorizar = new Autorizar();
        capaAutorizacaoNfse.autorizar.data_emissao = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
        capaAutorizacaoNfse.autorizar.natureza_operacao = "1";
        capaAutorizacaoNfse.autorizar.optante_simples_nacional = "false";
        capaAutorizacaoNfse.autorizar.prestador = new Prestador();
        capaAutorizacaoNfse.autorizar.prestador.cnpj = row["cnpj"].ToString();
        capaAutorizacaoNfse.autorizar.prestador.inscricao_municipal = row["inscricao_municipal"].ToString();
        capaAutorizacaoNfse.autorizar.prestador.codigo_municipio = row["codigo_municipio_ibge"].ToString();
        capaAutorizacaoNfse.autorizar.tomador = new Tomador();
        if (row["nota_fiscal_cpf"].ToString().Length > 11)
          capaAutorizacaoNfse.autorizar.tomador.cnpj = row["nota_fiscal_cpf"].ToString();
        else
          capaAutorizacaoNfse.autorizar.tomador.cpf = row["nota_fiscal_cpf"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.razao_social = row["nota_fiscal_nome"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.email = row["nota_fiscal_email"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco = new Endereco();
        capaAutorizacaoNfse.autorizar.tomador.endereco.logradouro = row["nota_fiscal_endereco"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.numero = row["nota_fiscal_numero"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.complemento = row["nota_fiscal_complemento"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.bairro = row["nota_fiscal_bairro"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.codigo_municipio = row["codigo_municipio_ibge"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.uf = row["nota_fiscal_uf"].ToString();
        capaAutorizacaoNfse.autorizar.tomador.endereco.cep = row["nota_fiscal_cep"].ToString();
        capaAutorizacaoNfse.autorizar.servico = new Servico();
        capaAutorizacaoNfse.autorizar.servico.aliquota = "3.00";
        capaAutorizacaoNfse.autorizar.servico.discriminacao = "Nota fiscal referente a serviços prestados";
        capaAutorizacaoNfse.autorizar.servico.iss_retido = "false";
        capaAutorizacaoNfse.autorizar.servico.valor_iss = "0";
        capaAutorizacaoNfse.autorizar.servico.item_lista_servico = "0801";
        capaAutorizacaoNfse.autorizar.servico.codigo_tributario_municipio = "080101";
        capaAutorizacaoNfse.autorizar.servico.valor_servicos = row["valor_pagamento"].ToString().Replace(',', '.');
        capaAutorizacaoNfse.identificador_nota = Convert.ToInt32(row["id_faturamento"]);
      }
      return this.AutorizarNfse(capaAutorizacaoNfse);
    }

    public PrestadorAcesso VerificarPrestador(string cnpj, bool _isdev,CapaAutorizacaoNfse obj)
    {
      PrestadorAcesso prestadorAcesso = new PrestadorAcesso();
      
      if (_isdev)
      {
        prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
        prestadorAcesso._base = "db_NfseDev";
        prestadorAcesso.server = "http://homologacao.acrasnfe.acras.com.br/";
      }
      else
      {
        prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
        prestadorAcesso._base = "db_Nfse";
        prestadorAcesso.server = "https://api.focusnfe.com.br/";
      }
      
      ConnectionFactory.connectionString = prestadorAcesso.connection;
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
                                           WHERE  prestador_cnpj='{0}'
						                          and b.item_lista_servico = '{2}'
                                                  and a.prestador_codigo_municipio_ibge = '{3}'", (object)cnpj, (object)prestadorAcesso._base,obj.autorizar.servico.item_lista_servico, obj.autorizar.prestador.codigo_municipio).ToString(), false);
      
      if (dataTable == null)
        return prestadorAcesso;
      foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
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
        PrestadorAcesso prestadorAcesso = new PrestadorAcesso();

        if (_isdev)
        {
            prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
            prestadorAcesso._base = "db_NfseDev";
            prestadorAcesso.server = "http://homologacao.acrasnfe.acras.com.br/";
        }
        else
        {
            prestadorAcesso.connection = "Data Source=179.107.47.90;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=ws_patio;Password=Studio55#";
            prestadorAcesso._base = "db_Nfse";
            prestadorAcesso.server = "https://api.focusnfe.com.br/";
        }

        ConnectionFactory.connectionString = prestadorAcesso.connection;
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
            return prestadorAcesso;
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
      PrestadorAcesso prestadorAcesso = this.VerificarPrestador(obj.cnpj_prestador.Replace("/", "").Replace(".", "").Replace("-", ""), obj.homologacao);
      if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
        return (RetornoConsulta) null;
      string nfse = new Tools().GetNfse(prestadorAcesso.server + "v2/nfse/" + obj.referencia, prestadorAcesso.prestador_chave);
      try
      {
        return this.InserirConsulta_obj(prestadorAcesso, obj, nfse);
      }
      catch
      {
        return (RetornoConsulta) null;
      }
    }
  }
}
