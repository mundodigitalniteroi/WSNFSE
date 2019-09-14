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

            SQL.AppendLine("INSERT INTO " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_consultar_dp_nf");

            SQL.AppendLine("    (id_autorizacao_nf");
            SQL.AppendLine("    ,id_usuario");
            SQL.AppendLine("    ,status_nf");
            SQL.AppendLine("    ,numero_nf");
            SQL.AppendLine("    ,codigo_verificacao");
            SQL.AppendLine("    ,data_emissao");
            SQL.AppendLine("    ,url_nota_fiscal");

            if (model.ImagemNotaFiscal != null)
            {
                SQL.AppendLine("    ,imagem_nota_fiscal");
            }

            SQL.AppendLine("    ,caminho_xml_nota_fiscal)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("    (@id_autorizacao_nf");
            SQL.AppendLine("    ,@id_usuario");
            SQL.AppendLine("    ,@status_nf");
            SQL.AppendLine("    ,@numero_nf");
            SQL.AppendLine("    ,@codigo_verificacao");
            SQL.AppendLine("    ,@data_emissao");
            SQL.AppendLine("    ,@url_nota_fiscal");

            if (model.ImagemNotaFiscal != null)
            {
                SQL.AppendLine("    ,@imagem_nota_fiscal");
            }

            SQL.AppendLine("    ,@caminho_xml_nota_fiscal)");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@id_autorizacao_nf",SqlDbType.Int) { Value = model.AutorizacaoNotaFiscalId },
                new SqlParameter("@id_usuario",SqlDbType.Int) { Value = model.UsuarioId },
                new SqlParameter("@status_nf",SqlDbType.VarChar) { Value = model.StatusNotaFiscal },
                new SqlParameter("@numero_nf",SqlDbType.VarChar) {Value = model.NumeroNotaFiscal },
                new SqlParameter("@codigo_verificacao",SqlDbType.VarChar) { Value = model.CodigoVerificacao },
                new SqlParameter("@data_emissao",SqlDbType.DateTime) { Value = model.DataEmissao },
                new SqlParameter("@url_nota_fiscal",SqlDbType.VarChar) { Value = model.UrlNotaFiscal },
                new SqlParameter("@caminho_xml_nota_fiscal",SqlDbType.VarChar) { Value = model.CaminhoNotaFiscal }
            };

            if (model.ImagemNotaFiscal != null)
            {
                sqlParameters = DataBase.AddNewParameter(sqlParameters, "@imagem_nota_fiscal", model.ImagemNotaFiscal, SqlDbType.VarBinary, model.ImagemNotaFiscal.Length);
            }

            return DataBase.ExecuteScalar(SQL, sqlParameters);
        }
    }
}