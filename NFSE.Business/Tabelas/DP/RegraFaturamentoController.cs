using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class RegraFaturamentoController
    {
        public List<RegraFaturamentoEntity> Listar(int clienteId, int depositoId, int faturamentoRegraTipoId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_faturamento_regras.id_faturamento_regra AS FaturamentoRegraId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.id_faturamento_regra_tipo AS FaturamentoRegraTipoId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.id_cliente AS ClienteId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.id_deposito AS DepositoId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.valor AS Valor");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.data_vigencia_inicial AS DataVigenciaInicial");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.data_vigencia_final AS DataVigenciaFinal");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_faturamento_regras.data_alteracao AS DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_dep_faturamento_regras");

            SQL.Append(" WHERE tb_dep_faturamento_regras.id_cliente = ").Append(clienteId).AppendLine();

            SQL.Append("   AND tb_dep_faturamento_regras.id_deposito = ").Append(depositoId).AppendLine();

            SQL.Append("   AND tb_dep_faturamento_regras.id_faturamento_regra_tipo = ").Append(faturamentoRegraTipoId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<RegraFaturamentoEntity>(dataTable);
            }
        }

        public RegraFaturamentoEntity Selecionar(int clienteId, int depositoId, int faturamentoRegraTipoId)
        {
            var list = Listar(clienteId, depositoId, faturamentoRegraTipoId);

            return list?.FirstOrDefault();
        }
    }
}