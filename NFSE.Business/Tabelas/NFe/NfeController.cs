using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeController
    {
        public List<NfeEntity> ListarPorIdentificadorNota(int identificadorNota)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT NfeID AS NfeId");
            SQL.AppendLine("      ,GrvID AS GrvId");
            SQL.AppendLine("      ,UsuarioCadastroID AS UsuarioCadastroId");
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
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public List<NfeEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT NfeID AS NfeId");
            SQL.AppendLine("      ,GrvID AS GrvId");
            SQL.AppendLine("      ,UsuarioCadastroID AS UsuarioCadastroId");
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

            SQL.AppendLine(" WHERE GrvId = " + grvId);

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public NfeEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe");

            SQL.AppendLine("      (GrvID");
            SQL.AppendLine("      ,UsuarioCadastroID");
            SQL.AppendLine("      ,Cnpj");
            SQL.AppendLine("      ,IdentificadorNota)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      (" + model.GrvId);
            SQL.AppendLine("      ," + model.UsuarioCadastroId);
            SQL.AppendLine("      ,'" + model.Cnpj + "'");
            SQL.AppendLine("      ," + model.IdentificadorNota + ")");

            return DataBase.ExecuteScalar(SQL);
        }

        public int Atualizar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.AppendLine("   SET Status = '" + model.Status + "'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.AppendLine("       ,CodigoRetorno = " + model.CodigoRetorno);
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE NfeID = " + model.NfeId);

            return DataBase.Execute(SQL);
        }
    }
}