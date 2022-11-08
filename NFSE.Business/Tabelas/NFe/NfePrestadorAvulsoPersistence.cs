using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public abstract class NfePrestadorAvulsoPersistence
    {
        public static List<NfePrestadorAvulsoEntity> Listar(NfePrestadorAvulsoEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT NfePrestadorAvulsoId");

            SQL.AppendLine("      ,Cnpj");

            SQL.AppendLine("      ,Nome");

            SQL.AppendLine("      ,Token");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_prestador_avulso");

            if (model != null)
            {
                SQL.AppendLine(" WHERE 1 = 1");

                if (model.NfePrestadorAvulsoId > 0)
                {
                    SQL.Append("   AND NfeRegraId = ").Append(model.NfePrestadorAvulsoId).AppendLine();
                }

                if (string.IsNullOrWhiteSpace(model.Cnpj))
                {
                    SQL.Append("   AND Cnpj = '").Append(model.Cnpj).AppendLine("'");
                }
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfePrestadorAvulsoEntity>(dataTable);
            }
        }

        public static NfePrestadorAvulsoEntity Selecionar(NfePrestadorAvulsoEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}
