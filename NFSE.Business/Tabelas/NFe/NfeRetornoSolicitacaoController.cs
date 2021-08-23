using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeRetornoSolicitacaoController
    {
        public List<NfeRetornoSolicitacaoEntity> Listar(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_retorno_solicitacao.RetornoSolicitacaoId");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.NfeId");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.NfePrestadorId");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.NaturezaOperacao");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.OptanteSimplesNacional");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorCpfCnpj");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorCnpj");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorNomeRazaoSocial");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorTelefone");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEmail");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoLogradouro");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoNumero");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoComplemento");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoBairro");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoCodigoMunicipio");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoUf");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.TomadorEnderecoCep");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoAliquota");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoDiscriminacao");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoIssRetido");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoValorIss");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoCodigoCnae");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoItemListaServico");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoValorServicos");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.ServicoCodigoTributarioMunicipio");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.RespostaEnvio");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.DataEmissao");

            SQL.AppendLine("      ,tb_dep_nfe_retorno_solicitacao.DataCadastro");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_retorno_solicitacao");

            SQL.Append("   AND tb_dep_nfe_retorno_solicitacao.NfeId = ").Append(nfeId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeRetornoSolicitacaoEntity>(dataTable);
            }
        }

        public NfeRetornoSolicitacaoEntity Selecionar(int nfeId)
        {
            var list = Listar(nfeId);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeEntity nfe, CapaAutorizacaoNfse capaAutorizacaoNfse, string resposta, string json)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_retorno_solicitacao");

            SQL.AppendLine("      (NfeId");
            SQL.AppendLine("      ,NaturezaOperacao");
            SQL.AppendLine("      ,OptanteSimplesNacional");
            SQL.AppendLine("      ,TomadorCpfCnpj");
            SQL.AppendLine("      ,TomadorCnpj");
            SQL.AppendLine("      ,TomadorNomeRazaoSocial");
            SQL.AppendLine("      ,TomadorTelefone");
            SQL.AppendLine("      ,TomadorEmail");
            SQL.AppendLine("      ,TomadorEnderecoLogradouro");
            SQL.AppendLine("      ,TomadorEnderecoNumero");
            SQL.AppendLine("      ,TomadorEnderecoComplemento");
            SQL.AppendLine("      ,TomadorEnderecoBairro");
            SQL.AppendLine("      ,TomadorEnderecoCodigoMunicipio");
            SQL.AppendLine("      ,TomadorEnderecoUf");
            SQL.AppendLine("      ,TomadorEnderecoCep");
            SQL.AppendLine("      ,ServicoAliquota");
            SQL.AppendLine("      ,ServicoDiscriminacao");
            SQL.AppendLine("      ,ServicoIssRetido");
            SQL.AppendLine("      ,ServicoValorIss");
            SQL.AppendLine("      ,ServicoCodigoCnae");
            SQL.AppendLine("      ,ServicoItemListaServico");
            SQL.AppendLine("      ,ServicoValorServicos");
            SQL.AppendLine("      ,ServicoCodigoTributarioMunicipio");
            SQL.AppendLine("      ,RespostaEnvio");
            SQL.AppendLine("      ,Json");
            SQL.AppendLine("      ,DataEmissao)");

            SQL.AppendLine("VALUES");

            SQL.Append("      (").Append(nfe.NfeId).AppendLine();
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.natureza_operacao));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(Convert.ToInt32(bool.Parse(capaAutorizacaoNfse.Autorizacao.optante_simples_nacional)).ToString()));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cpf));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cnpj));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.razao_social));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.telefone));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.email));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.logradouro));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.numero));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.complemento));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.bairro));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.codigo_municipio));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.uf));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.cep));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.aliquota));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.discriminacao));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.Autorizacao.servico.iss_retido)).ToString()));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_iss));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.codigo_cnae));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_servicos));
            SQL.Append("      ,").AppendLine(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.codigo_tributario_municipio));
            SQL.AppendLine("      ,@RespostaEnvio");
            SQL.AppendLine("      ,@Json");
            SQL.Append("      ,").Append(DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.data_emissao)).AppendLine(")");

            var sqlParameters = new SqlParameter[2];

            sqlParameters[0] = new SqlParameter("@RespostaEnvio", SqlDbType.VarChar)
            {
                Value = resposta
            };

            sqlParameters[1] = new SqlParameter("@Json", SqlDbType.VarChar)
            {
                Value = json
            };

            return DataBase.ExecuteScopeIdentity(SQL, sqlParameters);
        }
    }
}