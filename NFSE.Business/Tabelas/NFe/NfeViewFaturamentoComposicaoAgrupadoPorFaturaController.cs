using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeViewFaturamentoComposicaoAgrupadoPorFaturaController
    {
        public List<NfeViewFaturamentoComposicaoAgrupadoEntity> Listar(int faturamentoId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.GrvId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.AtendimentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.FaturamentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.CnaeId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.Cnae");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.ListaServicoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.Servico");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.FlagEnviarValorIss");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.FlagEnviarInscricaoEstadual");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.TotalComDesconto");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento");

            SQL.Append(" WHERE vw_dep_nfe_faturamento_composicao_agrupado_por_faturamento.FaturamentoId = ").Append(faturamentoId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfeViewFaturamentoComposicaoAgrupadoEntity>(dataTable);
            }
        }

        public NfeViewFaturamentoComposicaoAgrupadoEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }
    }
}