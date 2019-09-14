using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class FaturamentoController
    {
        public List<FaturamentoEntity> Listar(int atendimentoId, char status)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_faturamento.id_faturamento AS FaturamentoId");

            SQL.AppendLine("      ,tb_dep_faturamento.id_atendimento AS AtendimentoId");

            SQL.AppendLine("      ,tb_dep_faturamento.id_tipo_meio_cobranca AS TipoMeioCobrancaId");

            SQL.AppendLine("      ,tb_dep_faturamento.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_faturamento.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_faturamento.numero_identificacao AS NumeroIdentificacao");

            SQL.AppendLine("      ,tb_dep_faturamento.valor_faturado AS ValorFaturado");

            SQL.AppendLine("      ,tb_dep_faturamento.valor_pagamento AS ValorPagamento");

            SQL.AppendLine("      ,tb_dep_faturamento.hora_diaria AS HoraDiaria");

            SQL.AppendLine("      ,tb_dep_faturamento.maximo_diarias_para_cobranca AS MaximoDiariasParaCobranca");

            SQL.AppendLine("      ,tb_dep_faturamento.maximo_dias_vencimento AS MaximoDiasVencimento");

            SQL.AppendLine("      ,tb_dep_faturamento.sequencia AS Sequencia");

            SQL.AppendLine("      ,tb_dep_faturamento.numero_nota_fiscal AS NumeroNotaFiscal");

            SQL.AppendLine("      ,tb_dep_faturamento.data_calculo AS DataCalculo");

            SQL.AppendLine("      ,tb_dep_faturamento.data_retroativa AS DataRetroativa");

            SQL.AppendLine("      ,tb_dep_faturamento.data_vencimento AS DataVencimento");

            SQL.AppendLine("      ,tb_dep_faturamento.data_prazo_retirada_veiculo AS DataPrazoRetiradaVeiculo");

            SQL.AppendLine("      ,tb_dep_faturamento.data_pagamento AS DataPagamento");

            SQL.AppendLine("      ,tb_dep_faturamento.data_emissao_documento AS DataEmissaoDocumento");

            SQL.AppendLine("      ,tb_dep_faturamento.data_emissao_nota_fiscal AS DataEmissaoNotaFiscal");

            SQL.AppendLine("      ,tb_dep_faturamento.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_faturamento.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_faturamento.status AS Status");

            SQL.AppendLine("      ,tb_dep_faturamento.flag_usar_hora_diaria AS FlagUsarHoraDiaria");

            SQL.AppendLine("      ,tb_dep_faturamento.flag_limitacao_judicial AS FlagLimitacaoJudicial");

            SQL.AppendLine("      ,tb_dep_faturamento.flag_cliente_realiza_faturamento_arrecadacao AS FlagClienteRealizaFaturamentoArrecadacao");

            SQL.AppendLine("      ,tb_dep_faturamento.flag_cobrar_diarias_dias_corridos AS FlagCobrarDiariasDiasCorridos");

            SQL.AppendLine("      ,tb_dep_faturamento.flag_permissao_data_retroativa_faturamento AS FlagPermissaoDataRetroativaFaturamento");

            SQL.AppendLine("  FROM dbo.tb_dep_faturamento");

            SQL.AppendLine(" WHERE tb_dep_faturamento.id_atendimento = " + atendimentoId);

            SQL.AppendLine("   AND tb_dep_faturamento.status = '" + status + "'");

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<FaturamentoEntity>(dataTable);
            }
        }

        public FaturamentoEntity Selecionar(int atendimentoId, char status)
        {
            var list = Listar(atendimentoId, status);

            return list?.FirstOrDefault();
        }
    }
}