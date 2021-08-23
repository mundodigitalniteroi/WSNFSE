using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeViewFaturamentoComposicaoAgrupadoDescricaoController
    {
        public List<NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_faturamento_composicao_agrupado_descricao.NumeroFormularioGrv");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.GrvId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.AtendimentoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.CnaeId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.Cnae");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.ListaServicoId");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.Servico");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.DescricaoConfiguracaoNfe");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.FlagEnviarValorIss");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.FlagEnviarInscricaoEstadual");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.QuantidadeComposicao");
            
            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.ValorTipoComposicao");
            
            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.ValorCalculadoSemDesconto");
            
            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.TipoDesconto");
            
            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.QuantidadeDesconto");
            
            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.ValorDesconto");

            SQL.AppendLine("      ,vw_dep_nfe_faturamento_composicao_agrupado_descricao.TotalComDesconto");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_faturamento_composicao_agrupado_descricao");

            SQL.Append(" WHERE vw_dep_nfe_faturamento_composicao_agrupado_descricao.GrvId = ").Append(grvId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity>(dataTable);
            }
        }

        public NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }
    }
}