using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class AtendimentoController
    {
        public List<AtendimentoEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_atendimento.id_atendimento AS AtendimentoId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_grv AS GrvId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_qualificacao_responsavel AS QualificacaoResponsavelId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_pessoa_faturamento AS PessoaFaturamentoId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_empresa_faturamento AS EmpresaFaturamentoId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_documento_sap AS DocumentoSapId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_atendimento.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_nome AS ResponsavelNome");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_documento AS ResponsavelDocumento");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_cnh AS ResponsavelCnh");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_endereco AS ResponsavelEndereco");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_numero AS ResponsavelNumero");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_complemento AS ResponsavelComplemento");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_bairro AS ResponsavelBairro");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_municipio AS ResponsavelMunicipio");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_uf AS ResponsavelUf");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_cep AS ResponsavelCep");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_ddd AS ResponsavelDdd");

            SQL.AppendLine("      ,tb_dep_atendimento.responsavel_telefone AS ResponsavelTelefone");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_nome AS ProprietarioNome");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_id_tipo_documento AS ProprietarioIdTipoDocumento");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_documento AS ProprietarioDocumento");

            SQL.AppendLine("      ,tb_dep_atendimento.forma_liberacao AS FormaLiberacao");

            SQL.AppendLine("      ,tb_dep_atendimento.forma_liberacao_nome AS FormaLiberacaoNome");

            SQL.AppendLine("      ,tb_dep_atendimento.forma_liberacao_cnh AS FormaLiberacaoCnh");

            SQL.AppendLine("      ,tb_dep_atendimento.forma_liberacao_cpf AS FormaLiberacaoCpf");

            SQL.AppendLine("      ,tb_dep_atendimento.forma_liberacao_placa AS FormaLiberacaoPlaca");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_endereco AS ProprietarioEndereco");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_numero AS ProprietarioNumero");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_complemento AS ProprietarioComplemento");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_bairro AS ProprietarioBairro");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_municipio AS ProprietarioMunicipio");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_uf AS ProprietarioUf");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_cep AS ProprietarioCep");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_ddd AS ProprietarioDdd");

            SQL.AppendLine("      ,tb_dep_atendimento.proprietario_telefone AS ProprietarioTelefone");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_nome AS NotaFiscalNome");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_cpf AS NotaFiscalCpf");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_endereco AS NotaFiscalEndereco");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_numero AS NotaFiscalNumero");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_complemento AS NotaFiscalComplemento");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_bairro AS NotaFiscalBairro");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_municipio AS NotaFiscalMunicipio");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_uf AS NotaFiscalUf");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_cep AS NotaFiscalCep");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_ddd AS NotaFiscalDdd");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_telefone AS NotaFiscalTelefone");

            SQL.AppendLine("      ,tb_dep_atendimento.nota_fiscal_email AS NotaFiscalEmail");

            SQL.AppendLine("      ,tb_dep_atendimento.total_impressoes AS TotalImpressoes");

            SQL.AppendLine("      ,tb_dep_atendimento.status_cadastro_sap AS StatusCadastroSap");

            SQL.AppendLine("      ,tb_dep_atendimento.status_cadastro_ordens_venda_sap AS StatusCadastroOrdensVendaSap");

            SQL.AppendLine("      ,tb_dep_atendimento.data_hora_inicio_atendimento AS DataHoraInicioAtendimento");

            SQL.AppendLine("      ,tb_dep_atendimento.data_impressao AS DataImpressao");

            SQL.AppendLine("      ,tb_dep_atendimento.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_atendimento.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_atendimento.flag_pagamento_financiado AS FlagPagamentoFinanciado");

            SQL.AppendLine("      ,tb_dep_atendimento.flag_atendimento_ws AS FlagAtendimentoWs");

            SQL.AppendLine("  FROM dbo.tb_dep_atendimento");

            SQL.AppendLine(" WHERE tb_dep_atendimento.id_grv = " + grvId);

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<AtendimentoEntity>(dataTable);
            }
        }

        public AtendimentoEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }
    }
}