using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeViewFaturamentoComposicaoPorFaturaController
    {
        public List<NfeViewFaturamentoComposicaoEntity> Listar(int grvId, int nfeId = 0, int faturamentoId = 0)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_faturamento_composicao_por_fatura.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.GrvId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.AtendimentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.FaturamentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.CnaeId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.Cnae");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.ListaServicoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.Servico");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.DescricaoConfiguracaoNfe");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_por_fatura.FaturamentoComposicaoId");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_faturamento_composicao_por_fatura");

            if (nfeId > 0)
            {
                SQL.AppendLine("  JOIN dbo.tb_dep_nfe_faturamento_composicao");
                SQL.AppendLine("    ON tb_dep_nfe_faturamento_composicao.FaturamentoComposicaoId = vw_dep_nfe_faturamento_composicao_por_fatura.FaturamentoComposicaoId");
                SQL.Append("   AND tb_dep_nfe_faturamento_composicao.NfeId = ").Append(nfeId).AppendLine();
            }

            SQL.AppendLine(" WHERE 1 = 1");

            if (faturamentoId > 0)
            {
                SQL.Append("   AND vw_dep_nfe_faturamento_composicao_por_fatura.FaturamentoId = ").Append(faturamentoId).AppendLine();
            }

            SQL.Append("   AND vw_dep_nfe_faturamento_composicao_por_fatura.GrvId = ").Append(grvId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<NfeViewFaturamentoComposicaoEntity>(dataTable);
            }
        }
        public NfeViewFaturamentoComposicaoEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }
    }
}