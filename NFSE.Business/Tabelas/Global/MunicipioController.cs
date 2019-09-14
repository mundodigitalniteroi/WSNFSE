using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class MunicipioController
    {
        public List<MunicipioEntity> Listar(string uf)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_glo_loc_municipios.id_municipio AS MunicipioId");

            SQL.AppendLine("      ,tb_glo_loc_municipios.uf AS Uf");

            SQL.AppendLine("      ,tb_glo_loc_municipios.nome AS Nome");

            SQL.AppendLine("      ,tb_glo_loc_municipios.nome_ptbr AS NomePtbr");

            SQL.AppendLine("      ,tb_glo_loc_municipios.codigo_municipio AS CodigoMunicipio");

            SQL.AppendLine("      ,tb_glo_loc_municipios.codigo_municipio_ibge AS CodigoMunicipioIbge");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_municipios");

            SQL.AppendLine(" WHERE tb_glo_loc_municipios.uf = '" + uf + "'");

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<MunicipioEntity>(dataTable);
            }
        }

        public MunicipioEntity Selecionar(string uf)
        {
            var list = Listar(uf);

            return list?.FirstOrDefault();
        }

        public string SelecionarPrimeiroCodigoIbge(string uf)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT MIN(codigo_municipio_ibge) AS CodigoMunicipioIbge");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_municipios");

            SQL.AppendLine(" WHERE uf = '" + uf + "'");
            SQL.AppendLine("   AND codigo_municipio_ibge IS NOT NULL");

            using (var result = DataBase.Select(SQL))
            {
                if (result == null)
                {
                    return string.Empty;
                }

                return result.Rows[0]["CodigoMunicipioIbge"].ToString();
            }
        }
    }
}