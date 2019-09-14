using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class FaturamentoAssociadoCnaeController
    {
        public List<FaturamentoAssociadoCnaeEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_faturamento_associado_cnae.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.GrvId");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.AtendimentoId");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.CnaeId");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.Cnae");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.ListaServicoId");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.Servico");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.DescricaoConfiguracaoNfe");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.TotalComDesconto");

            SQL.AppendLine("      ,vw_faturamento_associado_cnae.FlagEnviarValorIss");

            SQL.AppendLine("  FROM dbo.vw_faturamento_associado_cnae");

            SQL.AppendLine(" WHERE vw_faturamento_associado_cnae.GrvId = " + grvId);

            using (var dataTable = DataBase.Select(SQL))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<FaturamentoAssociadoCnaeEntity>(dataTable);
            }
        }

        public FaturamentoAssociadoCnaeEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }
    }
}