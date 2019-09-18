using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeRetornoController
    {
        public int Cadastrar(NfeRetornoModel model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_nota_fiscal");

            SQL.AppendLine("    (NfeId");
            SQL.AppendLine("    ,UsuarioId");
            SQL.AppendLine("    ,Status");
            SQL.AppendLine("    ,NumeroNotaFiscal");
            SQL.AppendLine("    ,CodigoVerificacao");
            SQL.AppendLine("    ,UrlNotaFiscal");
            SQL.AppendLine("    ,CaminhoXmlNotaFiscal");

            if (model.ImagemNotaFiscal != null)
            {
                SQL.AppendLine("    ,ImagemNotaFiscal");
            }

            SQL.AppendLine("    ,DataEmissao)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("    (@NfeId");
            SQL.AppendLine("    ,@UsuarioId");
            SQL.AppendLine("    ,@Status");
            SQL.AppendLine("    ,@NumeroNotaFiscal");
            SQL.AppendLine("    ,@CodigoVerificacao");
            SQL.AppendLine("    ,@UrlNotaFiscal");
            SQL.AppendLine("    ,@CaminhoXmlNotaFiscal");

            if (model.ImagemNotaFiscal != null)
            {
                SQL.AppendLine("    ,@ImagemNotaFiscal");
            }

            SQL.AppendLine("    ,@DataEmissao)");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@NfeId",SqlDbType.Int) { Value = model.NfeId },
                new SqlParameter("@UsuarioId",SqlDbType.Int) { Value = model.UsuarioId },
                new SqlParameter("@Status",SqlDbType.VarChar) { Value = model.Status },
                new SqlParameter("@NumeroNotaFiscal",SqlDbType.VarChar) {Value = model.NumeroNotaFiscal },
                new SqlParameter("@CodigoVerificacao",SqlDbType.VarChar) { Value = model.CodigoVerificacao },
                new SqlParameter("@UrlNotaFiscal",SqlDbType.VarChar) { Value = model.UrlNotaFiscal },
                new SqlParameter("@CaminhoXmlNotaFiscal",SqlDbType.VarChar) { Value = model.CaminhoXmlNotaFiscal },
                new SqlParameter("@DataEmissao",SqlDbType.DateTime) { Value = model.DataEmissao }
            };

            if (model.ImagemNotaFiscal != null)
            {
                sqlParameters = DataBase.AddNewParameter(sqlParameters, "@ImagemNotaFiscal", model.ImagemNotaFiscal, SqlDbType.VarBinary, model.ImagemNotaFiscal.Length);
            }

            return DataBase.ExecuteScopeIdentity(SQL, sqlParameters);
        }
    }
}