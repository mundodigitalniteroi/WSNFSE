using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeViewFaturamentoComposicaoController
    {
        public List<NfeViewFaturamentoComposicaoEntity> Listar(int grvId, int nfeId = 0)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_faturamento_composicao.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.GrvId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.AtendimentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.CnaeId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.Cnae");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.ListaServicoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.Servico");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.DescricaoConfiguracaoNfe");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao.FaturamentoComposicaoId");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_faturamento_composicao");

            if (nfeId > 0)
            {
                SQL.AppendLine("  JOIN dbo.tb_dep_nfe_faturamento_composicao");
                SQL.AppendLine("    ON tb_dep_nfe_faturamento_composicao.FaturamentoComposicaoId = vw_dep_nfe_faturamento_composicao.FaturamentoComposicaoId");
                SQL.AppendLine("   AND tb_dep_nfe_faturamento_composicao.NfeId = " + nfeId);
            }

            SQL.AppendLine(" WHERE vw_dep_nfe_faturamento_composicao.GrvId = " + grvId);

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