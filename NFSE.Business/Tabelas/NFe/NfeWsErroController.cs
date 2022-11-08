using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeWsErroController
    {
        public List<NfeWsErroModel> Listar(NfeWsErroModel model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT ErroId");
            SQL.AppendLine("      ,GrvId");
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

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.ErroId > 0)
            {
                SQL.Append("   AND ErroId = ").Append(model.ErroId).AppendLine();
            }

            if (model.GrvId > 0)
            {
                SQL.Append("   AND GrvId = ").Append(model.GrvId).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(model.IdentificadorNota))
            {
                SQL.Append("   AND IdentificadorNota = '").Append(model.IdentificadorNota).AppendLine("'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfeWsErroModel>(dataTable);
            }
        }

        public NfeWsErroModel Selecionar(NfeWsErroModel model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeWsErroModel model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_ws_erros");
            
            SQL.AppendLine("    (GrvId");
            SQL.AppendLine("    ,IdentificadorNota");
            SQL.AppendLine("    ,UsuarioId");
            SQL.AppendLine("    ,Acao");
            SQL.AppendLine("    ,OrigemErro");
            SQL.AppendLine("    ,Status");
            SQL.AppendLine("    ,CodigoErro");
            SQL.AppendLine("    ,MensagemErro");
            SQL.AppendLine("    ,CorrecaoErro)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("    (@GrvId");
            SQL.AppendLine("    ,@IdentificadorNota");
            SQL.AppendLine("    ,@UsuarioId");
            SQL.AppendLine("    ,@Acao");
            SQL.AppendLine("    ,@OrigemErro");
            SQL.AppendLine("    ,@Status");
            SQL.AppendLine("    ,@CodigoErro");
            SQL.AppendLine("    ,@MensagemErro");
            SQL.AppendLine("    ,@CorrecaoErro)");

            if (model.Status != null)
            {
                model.Status = model.Status.ToUpper().Trim();

                if (model.Status.Length > 30)
                {
                    model.Status = model.Status.Substring(0, 30);
                }
            }

            if (model.CodigoErro != null)
            {
                model.CodigoErro = model.CodigoErro.ToUpper().Trim();

                model.CodigoErro = model.CodigoErro.Trim();

                if (model.CodigoErro.Equals("---") || model.CodigoErro.Equals("..."))
                {
                    model.CodigoErro = string.Empty;
                }
                else if (model.CodigoErro.Length > 30)
                {
                    model.CodigoErro = model.CodigoErro.Substring(0, 30).Trim();
                }

            }

            if (model.MensagemErro != null)
            {
                model.MensagemErro = model.MensagemErro.Trim();

                if (model.MensagemErro.Length > 1000)
                {
                    model.MensagemErro = model.MensagemErro.Substring(0, 1000).Trim();
                }
            }

            if (model.CorrecaoErro != null)
            {
                model.CorrecaoErro = model.CorrecaoErro.Trim();

                if (model.CorrecaoErro.Equals("---") || model.CorrecaoErro.Equals("..."))
                {
                    model.CorrecaoErro = string.Empty;
                }
                else if (model.CorrecaoErro.Length > 1000)
                {
                    model.CorrecaoErro = model.CorrecaoErro.Substring(0, 1000).Trim();
                }
            }

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@GrvId", SqlDbType.Int) { Value = model.GrvId },

                new SqlParameter("@IdentificadorNota", SqlDbType.VarChar) { Value = model.IdentificadorNota },

                new SqlParameter("@UsuarioId", SqlDbType.Int) { Value = model.UsuarioId },

                new SqlParameter("@Acao", SqlDbType.Char) { Value = model.Acao },

                new SqlParameter("@OrigemErro", SqlDbType.Char) { Value = model.OrigemErro },

                new SqlParameter("@Status", SqlDbType.VarChar) { Value = model.Status },

                new SqlParameter("@CodigoErro", SqlDbType.VarChar) { Value = model.CodigoErro },

                new SqlParameter("@MensagemErro", SqlDbType.VarChar) { Value = model.MensagemErro },

                new SqlParameter("@CorrecaoErro", SqlDbType.VarChar) { Value = model.CorrecaoErro }
            };

            return DataBase.ExecuteScopeIdentity(SQL, sqlParameters);
        }

        public void CadastrarErroGenerico(int grvId, int usuarioId, string identificadorNota, OrigemErro origemErro, Acao acao, string mensagemErro)
        {
            var erro = new NfeWsErroModel
            {
                GrvId = grvId,
                IdentificadorNota = identificadorNota,
                UsuarioId = usuarioId,
                Acao = (char)acao,
                OrigemErro = (char)origemErro,
                MensagemErro = mensagemErro
            };

            try
            {
                int id = new NfeWsErroController().Cadastrar(erro);
            }
            catch { }
        }
    }
}