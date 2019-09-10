using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class NfeWsErroController
    {
        public int Cadastrar(NfeWsErroModel model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_ws_erros");

            SQL.AppendLine("    (IdentificadorNota");
            SQL.AppendLine("    ,UsuarioId");
            SQL.AppendLine("    ,Acao");
            SQL.AppendLine("    ,OrigemErro");
            SQL.AppendLine("    ,Status");
            SQL.AppendLine("    ,CodigoErro");
            SQL.AppendLine("    ,MensagemErro");
            SQL.AppendLine("    ,CorrecaoErro)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("    (@IdentificadorNota");
            SQL.AppendLine("    ,@UsuarioId");
            SQL.AppendLine("    ,@Acao");
            SQL.AppendLine("    ,@OrigemErro");
            SQL.AppendLine("    ,@Status");
            SQL.AppendLine("    ,@CodigoErro");
            SQL.AppendLine("    ,@MensagemErro");
            SQL.AppendLine("    ,@CorrecaoErro)");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@IdentificadorNota", SqlDbType.Int) { Value = model.IdentificadorNota },
                new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = model.UsuarioId },
                new SqlParameter("@Acao", SqlDbType.Char) { Value = model.Acao },
                new SqlParameter("@OrigemErro", SqlDbType.Char) { Value = model.OrigemErro },
                new SqlParameter("@Status", SqlDbType.VarChar) { Value = model.Status },
                new SqlParameter("@CodigoErro", SqlDbType.VarChar) { Value = model.CodigoErro },
                new SqlParameter("@MensagemErro", SqlDbType.VarChar) { Value = model.MensagemErro },
                new SqlParameter("@CorrecaoErro", SqlDbType.VarChar) { Value = model.CorrecaoErro }
            };

            return DataBase.Execute(SQL, sqlParameters);
        }
    }
}