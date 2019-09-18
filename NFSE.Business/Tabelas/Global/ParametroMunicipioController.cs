using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class ParametroMunicipioController
    {
        public List<ParametroMunicipioEntity> Listar(ParametroMunicipioEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_gov_parametro_municipio.ParametroMunicipioId");

            SQL.AppendLine("      ,tb_gov_parametro_municipio.CodigoCnae");

            SQL.AppendLine("      ,tb_gov_parametro_municipio.ItemListaServico");

            SQL.AppendLine("      ,tb_gov_parametro_municipio.CodigoMunicipioIbge");

            SQL.AppendLine("      ,tb_gov_parametro_municipio.CodigoTributarioMunicipio");

            SQL.AppendLine("      ,tb_gov_parametro_municipio.DataCadastro");

            SQL.AppendLine("  FROM db_global.dbo.tb_gov_parametro_municipio");

            SQL.AppendLine(" WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(model.CodigoCnae))
            {
                SQL.AppendLine("   AND tb_gov_parametro_municipio.CodigoCnae = '" + model.CodigoCnae + "'");
            }

            if (!string.IsNullOrWhiteSpace(model.ItemListaServico))
            {
                SQL.AppendLine("   AND tb_gov_parametro_municipio.ItemListaServico = '" + model.ItemListaServico + "'");
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoMunicipioIbge))
            {
                SQL.AppendLine("   AND tb_gov_parametro_municipio.CodigoMunicipioIbge = '" + model.CodigoMunicipioIbge + "'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<ParametroMunicipioEntity>(dataTable);
            }
        }

        public ParametroMunicipioEntity Selecionar(ParametroMunicipioEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}