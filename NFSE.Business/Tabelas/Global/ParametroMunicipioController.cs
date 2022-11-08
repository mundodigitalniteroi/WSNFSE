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
                SQL.Append("   AND tb_gov_parametro_municipio.CodigoCnae = '").Append(model.CodigoCnae).AppendLine("'");
            }

            if (!string.IsNullOrWhiteSpace(model.ItemListaServico))
            {
                SQL.Append("   AND tb_gov_parametro_municipio.ItemListaServico = '").Append(model.ItemListaServico).AppendLine("'");
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoMunicipioIbge))
            {
                SQL.Append("   AND tb_gov_parametro_municipio.CodigoMunicipioIbge = '").Append(model.CodigoMunicipioIbge).AppendLine("'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<ParametroMunicipioEntity>(dataTable);
            }
        }

        public ParametroMunicipioEntity Selecionar(ParametroMunicipioEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}