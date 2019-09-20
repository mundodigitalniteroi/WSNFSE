using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeImagemController
    {
        #region Cadastrar
        public void Cadastrar(int nfeID, byte[] imagem)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_imagens");

            SQL.AppendLine("      (NfeID");
            SQL.AppendLine("      ,Imagem)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      (" + nfeID);
            SQL.AppendLine("      ,@Imagem)");

            var sqlParameter = new SqlParameter[1];

            sqlParameter[0] = new SqlParameter("@Imagem", SqlDbType.VarBinary)
            {
                Value = imagem
            };

            DataBase.ExecuteScopeIdentity(SQL, sqlParameter);
        }
        #endregion Cadastrar

        #region Excluir
        public void Excluir(int nfeID)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("DELETE FROM dbo.tb_dep_nfe_imagens");

            SQL.AppendLine(" WHERE NfeID = " + nfeID);

            DataBase.Execute(SQL);
        }
        #endregion Excluir
    }
}