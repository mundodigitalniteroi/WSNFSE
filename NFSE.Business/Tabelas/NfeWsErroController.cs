using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class NfeWsErroController
    {
        public List<NfeWsErroModel> Selecionar(int erroId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT ErroId");
            SQL.AppendLine("      ,IdentificadorNota");
            SQL.AppendLine("      ,UsuarioId");
            SQL.AppendLine("      ,Acao");
            SQL.AppendLine("      ,OrigemErro");
            SQL.AppendLine("      ,Status");
            SQL.AppendLine("      ,CodigoErro");
            SQL.AppendLine("      ,MensagemErro");
            SQL.AppendLine("      ,CorrecaoErro");
            SQL.AppendLine("      ,DataHoraCadastro");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_ws_erros");

            SQL.AppendLine(" WHERE ErroId = " + erroId);

            using (var dataTable = DataBase.Select(SQL))
            {
                if (dataTable == null)
                {
                    return null;
                }

                return DataTableUtil.DataTableToList<NfeWsErroModel>(dataTable);
            }
        }

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
                new SqlParameter("@CodigoErro", SqlDbType.VarChar) { Value = model.CodigoErro.ToUpper().Trim().Substring(0, 30) },
                new SqlParameter("@MensagemErro", SqlDbType.VarChar) { Value = model.MensagemErro.Trim().Substring(0, 1000) },
                new SqlParameter("@CorrecaoErro", SqlDbType.VarChar) { Value = model.CorrecaoErro.Trim().Substring(0, 1000) }
            };

            return DataBase.Execute(SQL, sqlParameters);
        }
    }
}