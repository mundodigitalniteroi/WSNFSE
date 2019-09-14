using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class CepController
    {
        public List<CepEntity> Listar(CepEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_glo_loc_cep.id_cep AS CepId");

            SQL.AppendLine("      ,tb_glo_loc_cep.id_municipio AS MunicipioId");

            SQL.AppendLine("      ,tb_glo_loc_cep.id_bairro AS BairroId");

            SQL.AppendLine("      ,tb_glo_loc_cep.id_tipo_logradouro AS TipoLogradouroId");

            SQL.AppendLine("      ,tb_glo_loc_cep.cep AS Cep");

            SQL.AppendLine("      ,tb_glo_loc_cep.logradouro AS Logradouro");

            SQL.AppendLine("      ,tb_glo_loc_cep.flag_sanitizado AS FlagSanitizado");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_cep");

            if (model.CepId > 0)
            {
                SQL.AppendLine(" WHERE tb_glo_loc_cep.id_cep = " + model.CepId);
            }
            else
            {
                SQL.AppendLine(" WHERE tb_glo_loc_cep.cep = '" + model.Cep + "'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<CepEntity>(dataTable);
            }
        }

        public CepEntity Selecionar(int cepId)
        {
            var list = Listar(new CepEntity { CepId = cepId });

            return list?.FirstOrDefault();
        }

        public CepEntity Selecionar(string cep)
        {
            var list = Listar(new CepEntity { Cep = cep });

            return list?.FirstOrDefault();
        }
    }
}