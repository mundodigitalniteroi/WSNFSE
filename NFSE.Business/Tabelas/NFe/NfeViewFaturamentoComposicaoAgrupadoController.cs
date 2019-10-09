using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeViewFaturamentoComposicaoAgrupadoController
    {
        public List<NfeViewFaturamentoComposicaoAgrupadoEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_faturamento_composicao_agrupado.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.GrvId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.AtendimentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.CnaeId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.Cnae");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.ListaServicoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.Servico");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.FlagEnviarValorIss");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.FlagEnviarInscricaoEstadual");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado.TotalComDesconto");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_faturamento_composicao_agrupado");

            SQL.AppendLine(" WHERE vw_dep_nfe_faturamento_composicao_agrupado.GrvId = " + grvId);

            using (var dataTable = DataBase.Select(SQL))
            {
                if (dataTable == null)
                {
                    return null;
                }

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