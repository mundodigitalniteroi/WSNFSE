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

            SQL.AppendLine("   AND tb_dep_nfe_retorno_solicitacao.NfeId = " + nfeId);

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

        public int Cadastrar(NfeEntity nfe, CapaAutorizacaoNfse capaAutorizacaoNfse, string resposta)
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
            SQL.AppendLine("      ,DataEmissao)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      (" + nfe.NfeId);
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.natureza_operacao));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(Convert.ToInt32(bool.Parse(capaAutorizacaoNfse.Autorizacao.optante_simples_nacional)).ToString()));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cpf));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cnpj));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.razao_social));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.telefone));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.email));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.logradouro));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.numero));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.complemento));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.bairro));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.codigo_municipio));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.uf));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.cep));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.aliquota));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.discriminacao));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.Autorizacao.servico.iss_retido)).ToString()));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_iss));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.codigo_cnae));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_servicos));
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.servico.codigo_tributario_municipio));
            SQL.AppendLine("      ,@RespostaEnvio");
            SQL.AppendLine("      ," + DataBase.SetNullIfEmpty(capaAutorizacaoNfse.Autorizacao.data_emissao) + ")");

            var sqlParameters = new SqlParameter[1];

            sqlParameters[0] = new SqlParameter("@RespostaEnvio", SqlDbType.VarChar)
            {
                Value = resposta
            };

            return DataBase.ExecuteScopeIdentity(SQL, sqlParameters);
        }
    }
}