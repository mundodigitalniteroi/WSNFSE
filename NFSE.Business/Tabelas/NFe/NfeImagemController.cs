using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeImagemController
    {
        public List<NfeImagemEntity> Listar(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_imagens.NfeImagemID AS NfeImagemId");

            SQL.AppendLine("      ,tb_dep_nfe_imagens.NfeID AS NfeId");

            SQL.AppendLine("      ,tb_dep_nfe_imagens.Imagem");

            SQL.AppendLine("      ,tb_dep_nfe_imagens.DataCadastro");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_imagens");

            SQL.Append(" WHERE tb_dep_nfe_imagens.NfeId = ").Append(nfeId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfeImagemEntity>(dataTable);
            }
        }

        public NfeImagemEntity Selecionar(int nfeId)
        {
            var list = Listar(nfeId);

            return list?.FirstOrDefault();
        }

        #region Cadastrar
        public void Cadastrar(int nfeId, byte[] imagem)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_imagens");

            SQL.AppendLine("      (NfeID");
            SQL.AppendLine("      ,Imagem)");

            SQL.AppendLine("VALUES");

            SQL.Append("      (").Append(nfeId).AppendLine();
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

            SQL.Append(" WHERE NfeID = ").Append(nfeID).AppendLine();

            DataBase.Execute(SQL);
        }
        #endregion Excluir
    }
}