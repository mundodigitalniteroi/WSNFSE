using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class EstadoController
    {
        public List<EstadoEntity> Listar(string uf)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_glo_loc_estados.uf AS Uf");

            SQL.AppendLine("      ,tb_glo_loc_estados.pais_numcode AS PaisNumcode");

            SQL.AppendLine("      ,tb_glo_loc_estados.regiao AS Regiao");

            SQL.AppendLine("      ,tb_glo_loc_estados.nome AS Nome");

            SQL.AppendLine("      ,tb_glo_loc_estados.nome_ptbr AS NomePtbr");

            SQL.AppendLine("      ,tb_glo_loc_estados.capital AS Capital");

            SQL.AppendLine("      ,tb_glo_loc_estados.id_utc AS UtcId");

            SQL.AppendLine("      ,tb_glo_loc_estados.id_utc_verao AS UtcVeraoId");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_estados");

            SQL.Append(" WHERE tb_glo_loc_estados.uf = '").Append(uf).AppendLine("'");

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<EstadoEntity>(dataTable);
            }
        }

        public EstadoEntity Selecionar(string uf)
        {
            var list = Listar(uf);

            return list?.FirstOrDefault();
        }
    }
}