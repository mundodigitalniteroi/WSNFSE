using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class TipoRegraFaturamentoController
    {
        public List<TipoRegraFaturamentoEntity> Listar(string codigo)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_faturamento_regras_tipos.id_faturamento_regra_tipo AS FaturamentoRegraTipoId");

            SQL.AppendLine("      ,tb_dep_faturamento_regras_tipos.codigo AS Codigo");

            SQL.AppendLine("      ,tb_dep_faturamento_regras_tipos.descricao AS Descricao");

            SQL.AppendLine("      ,tb_dep_faturamento_regras_tipos.flag_possui_valor AS FlagPossuiValor");

            SQL.AppendLine("      ,tb_dep_faturamento_regras_tipos.flag_ativo AS FlagAtivo");

            SQL.AppendLine("  FROM dbo.tb_dep_faturamento_regras_tipos");

            SQL.AppendLine(" WHERE tb_dep_faturamento_regras_tipos.codigo = '" + codigo + "'");

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<TipoRegraFaturamentoEntity>(dataTable);
            }
        }

        public TipoRegraFaturamentoEntity Selecionar(string codigo)
        {
            var list = Listar(codigo);

            return list?.FirstOrDefault();
        }
    }
}