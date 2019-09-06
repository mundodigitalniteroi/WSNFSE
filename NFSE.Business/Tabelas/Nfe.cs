using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class Nfe
    {
        public List<NFSE.Domain.Entities.DP.Nfe> ConsultarPorGrv(int idGrv)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT NfeID");
            SQL.AppendLine("      ,GrvID");
            SQL.AppendLine("      ,UsuarioCadastroID");
            SQL.AppendLine("      ,Cnpj");
            SQL.AppendLine("      ,CodigoRetorno");
            SQL.AppendLine("      ,CodigoVerificacao");
            SQL.AppendLine("      ,DataEmissao");
            SQL.AppendLine("      ,StatusNfe");
            SQL.AppendLine("      ,Url");
            SQL.AppendLine("      ,Status");
            SQL.AppendLine("      ,DataCadastro");
            SQL.AppendLine("      ,DataAlteracao");
            SQL.AppendLine("      ,IdentificadorNota");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe");

            SQL.AppendLine(" WHERE GrvID = @GrvID");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@GrvID",SqlDbType.Int) { Value = idGrv }
            };

            using (var dataTable = DataBase.Select(SQL, sqlParameters))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<NFSE.Domain.Entities.DP.Nfe>(dataTable);
            }
        }

        public List<NFSE.Domain.Entities.DP.Nfe> ConsultarPorIdentificadorNota(int identificadorNota)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT NfeID");
            SQL.AppendLine("      ,GrvID");
            SQL.AppendLine("      ,UsuarioCadastroID");
            SQL.AppendLine("      ,Cnpj");
            SQL.AppendLine("      ,CodigoRetorno");
            SQL.AppendLine("      ,CodigoVerificacao");
            SQL.AppendLine("      ,DataEmissao");
            SQL.AppendLine("      ,StatusNfe");
            SQL.AppendLine("      ,Url");
            SQL.AppendLine("      ,Status");
            SQL.AppendLine("      ,DataCadastro");
            SQL.AppendLine("      ,DataAlteracao");
            SQL.AppendLine("      ,IdentificadorNota");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe");

            SQL.AppendLine(" WHERE IdentificadorNota = @IdentificadorNota");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@IdentificadorNota",SqlDbType.Int) { Value = identificadorNota }
            };

            using (var dataTable = DataBase.Select(SQL, sqlParameters))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<NFSE.Domain.Entities.DP.Nfe>(dataTable);
            }
        }
    }
}