using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System;
using System.Data;
using System.Text;

namespace Negocio.Tabelas
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

        public int Cadastrar(PrestadorAcesso prestadorAcesso, CapaAutorizacaoNfse capaAutorizacaoNfse, string resposta)
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

            SQL.AppendLine("      ('" + capaAutorizacaoNfse.identificador_nota + "'");
            SQL.AppendLine("      ,'" + resposta + "'");
            SQL.AppendLine("      ," + prestadorAcesso.id_nfse_prestador);
            SQL.AppendLine("      ," + capaAutorizacaoNfse.id_usuario);
            SQL.AppendLine("      ,'" + Convert.ToInt32(capaAutorizacaoNfse.homologacao) + "'");
            SQL.AppendLine("      ,'" + Convert.ToInt32(capaAutorizacaoNfse.homologacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.data_emissao) ? "" : capaAutorizacaoNfse.autorizar.data_emissao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.natureza_operacao) ? "" : capaAutorizacaoNfse.autorizar.natureza_operacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.optante_simples_nacional) ? "" : Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.autorizar.optante_simples_nacional)).ToString()) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.cpf) ? "" : capaAutorizacaoNfse.autorizar.tomador.cpf) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.cnpj) ? "" : capaAutorizacaoNfse.autorizar.tomador.cnpj) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.razao_social) ? "" : capaAutorizacaoNfse.autorizar.tomador.razao_social) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.telefone) ? "" : capaAutorizacaoNfse.autorizar.tomador.telefone) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.email) ? "" : capaAutorizacaoNfse.autorizar.tomador.email) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.logradouro) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.logradouro) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.numero) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.numero) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.complemento) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.complemento) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.bairro) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.bairro) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.codigo_municipio) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.codigo_municipio) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.uf) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.uf) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.tomador.endereco.cep) ? "" : capaAutorizacaoNfse.autorizar.tomador.endereco.cep) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.aliquota) ? "" : capaAutorizacaoNfse.autorizar.servico.aliquota) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.discriminacao) ? "" : capaAutorizacaoNfse.autorizar.servico.discriminacao) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.iss_retido) ? "" : Convert.ToInt32(Convert.ToBoolean(capaAutorizacaoNfse.autorizar.servico.iss_retido)).ToString()) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.valor_iss) ? "" : capaAutorizacaoNfse.autorizar.servico.valor_iss) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.codigo_cnae) ? "" : capaAutorizacaoNfse.autorizar.servico.codigo_cnae) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.item_lista_servico) ? "" : capaAutorizacaoNfse.autorizar.servico.item_lista_servico) + "'");
            SQL.AppendLine("      ,'" + (string.IsNullOrEmpty(capaAutorizacaoNfse.autorizar.servico.valor_servicos) ? "" : capaAutorizacaoNfse.autorizar.servico.valor_servicos) + "')");

            return DataBase.Execute(SQL);
        }
    }
}