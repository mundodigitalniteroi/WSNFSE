using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class CnaeListaServicoController
    {
        public List<CnaeListaServicoEntity> Listar(CnaeListaServicoEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_gov_cnae_lista_servico.CnaeListaServicoID AS CnaeListaServicoId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.CnaeID AS CnaeId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.CnaeCodigo AS CnaeCodigo");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.CnaeCodigoFormatado AS CnaeCodigoFormatado");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.CnaeDescricao AS CnaeDescricao");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.ListaServicoID AS ListaServicoId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.ListaServico AS ListaServico");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.ListaServicoDescricao AS ListaServicoDescricao");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico.AliquotaIss AS AliquotaIss");

            SQL.AppendLine("  FROM db_global.dbo.vw_gov_cnae_lista_servico");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.CnaeId > 0)
            {
                SQL.Append("   AND vw_gov_cnae_lista_servico.CnaeId = ").Append(model.CnaeId).AppendLine();
            }

            if (model.ListaServicoId > 0)
            {
                SQL.Append("   AND vw_gov_cnae_lista_servico.ListaServicoId = ").Append(model.ListaServicoId).AppendLine();
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<CnaeListaServicoEntity>(dataTable);
            }
        }

        public CnaeListaServicoEntity Selecionar(CnaeListaServicoEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}