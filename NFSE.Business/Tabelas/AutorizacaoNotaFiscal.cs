using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System;
using System.Data;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class AutorizacaoNotaFiscal
    {
        public DataTable Consultar(int referencia)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT id_autorizacao_nf");
            SQL.AppendLine("      ,id_grv");
            SQL.AppendLine("      ,id_nfse_prestador");
            SQL.AppendLine("      ,id_usuario");
            SQL.AppendLine("      ,referencia_externa");
            SQL.AppendLine("      ,flag_ambiente");
            SQL.AppendLine("      ,data_emissao");
            SQL.AppendLine("      ,natureza_operacao");
            SQL.AppendLine("      ,optante_simples_nacional");
            SQL.AppendLine("      ,tomador_cpf_cnpj");
            SQL.AppendLine("      ,tomador_cnpj");
            SQL.AppendLine("      ,tomador_nome_razao_social");
            SQL.AppendLine("      ,tomador_telefone");
            SQL.AppendLine("      ,tomador_email");
            SQL.AppendLine("      ,tomador_endereco_logradouro");
            SQL.AppendLine("      ,tomador_endereco_numero");
            SQL.AppendLine("      ,tomador_endereco_complemento");
            SQL.AppendLine("      ,tomador_endereco_bairro");
            SQL.AppendLine("      ,tomador_endereco_codigo_municipio");
            SQL.AppendLine("      ,tomador_endereco_uf");
            SQL.AppendLine("      ,tomador_endereco_cep");
            SQL.AppendLine("      ,servico_aliquota");
            SQL.AppendLine("      ,servico_discriminacao");
            SQL.AppendLine("      ,servico_iss_retido");
            SQL.AppendLine("      ,servico_valor_iss");
            SQL.AppendLine("      ,servico_codigo_cnae");
            SQL.AppendLine("      ,servico_item_lista_servico");
            SQL.AppendLine("      ,servico_valor_servicos");
            SQL.AppendLine("      ,resposta_envio");

            SQL.AppendLine("  FROM " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_autorizacao_dp_nf");

            SQL.AppendLine(" WHERE referencia_externa = " + referencia);

            return DataBase.Select(SQL);
        }

        public int Cadastrar(PrestadorServico prestadorAcesso, CapaAutorizacaoNfse capaAutorizacaoNfse, string resposta)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_autorizacao_dp_nf");

            SQL.AppendLine("      (referencia_externa");
            SQL.AppendLine("      ,resposta_envio");
            SQL.AppendLine("      ,id_nfse_prestador");
            SQL.AppendLine("      ,id_usuario");
            SQL.AppendLine("      ,flag_ambiente");
            SQL.AppendLine("      ,data_emissao");
            SQL.AppendLine("      ,natureza_operacao");
            SQL.AppendLine("      ,optante_simples_nacional");
            SQL.AppendLine("      ,tomador_cpf_cnpj");
            SQL.AppendLine("      ,tomador_cnpj");
            SQL.AppendLine("      ,tomador_nome_razao_social");
            SQL.AppendLine("      ,tomador_telefone");
            SQL.AppendLine("      ,tomador_email");
            SQL.AppendLine("      ,tomador_endereco_logradouro");
            SQL.AppendLine("      ,tomador_endereco_numero");
            SQL.AppendLine("      ,tomador_endereco_complemento");
            SQL.AppendLine("      ,tomador_endereco_bairro");
            SQL.AppendLine("      ,tomador_endereco_codigo_municipio");
            SQL.AppendLine("      ,tomador_endereco_uf");
            SQL.AppendLine("      ,tomador_endereco_cep");
            SQL.AppendLine("      ,servico_aliquota");
            SQL.AppendLine("      ,servico_discriminacao");
            SQL.AppendLine("      ,servico_iss_retido");
            SQL.AppendLine("      ,servico_valor_iss");
            SQL.AppendLine("      ,servico_codigo_cnae");
            SQL.AppendLine("      ,servico_item_lista_servico");
            SQL.AppendLine("      ,servico_valor_servicos)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      ('" + capaAutorizacaoNfse.IdentificadorNota + "'");
            SQL.AppendLine("      ,'" + resposta + "'");
            SQL.AppendLine("      ," + prestadorAcesso.id_nfse_prestador);
            SQL.AppendLine("      ," + capaAutorizacaoNfse.UsuarioId);
            SQL.AppendLine("      ,'" + Convert.ToInt32(capaAutorizacaoNfse.Homologacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.data_emissao) ? "" : capaAutorizacaoNfse.Autorizacao.data_emissao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.natureza_operacao) ? "" : capaAutorizacaoNfse.Autorizacao.natureza_operacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.optante_simples_nacional) ? "" : Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.Autorizacao.optante_simples_nacional)).ToString()) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cpf) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.cpf) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.cnpj) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.cnpj) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.razao_social) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.razao_social) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.telefone) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.telefone) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.email) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.email) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.logradouro) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.logradouro) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.numero) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.numero) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.complemento) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.complemento) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.bairro) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.bairro) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.codigo_municipio) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.codigo_municipio) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.uf) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.uf) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.tomador.endereco.cep) ? "" : capaAutorizacaoNfse.Autorizacao.tomador.endereco.cep) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.aliquota) ? "" : capaAutorizacaoNfse.Autorizacao.servico.aliquota) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.discriminacao) ? "" : capaAutorizacaoNfse.Autorizacao.servico.discriminacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.iss_retido) ? "" : Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.Autorizacao.servico.iss_retido)).ToString()) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_iss) ? "" : capaAutorizacaoNfse.Autorizacao.servico.valor_iss) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.codigo_cnae) ? "" : capaAutorizacaoNfse.Autorizacao.servico.codigo_cnae) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico) ? "" : capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.Autorizacao.servico.valor_servicos) ? "" : capaAutorizacaoNfse.Autorizacao.servico.valor_servicos) + "')");

            return DataBase.ExecuteScalar(SQL);
        }
    }
}