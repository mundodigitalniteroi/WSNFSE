using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Negocio.Tabelas
{
    public class NotaFiscal
    {
        public int Cadastrar(NFSE.Domain.Entities.NotaFiscal model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_consultar_dp_nf");

            SQL.AppendLine("    (id_autorizacao_nf");
            SQL.AppendLine("    ,id_usuario");
            SQL.AppendLine("    ,flag_ambiente");
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
            SQL.AppendLine("    ,@flag_ambiente");
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
                new SqlParameter("@flag_ambiente",SqlDbType.VarChar) {Value = model.FlagAmbiente },
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

            return DataBase.Execute(SQL, sqlParameters);
        }

        public int CadastrarErro(RetornoErro model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_consultar_dp_nf_erro");

            SQL.AppendLine("    (AutorizacaoNotaFiscalId");
            SQL.AppendLine("    ,UsuarioId");
            SQL.AppendLine("    ,CodigoErro");
            SQL.AppendLine("    ,MensagemErro)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("    (@AutorizacaoNotaFiscalId");
            SQL.AppendLine("    ,@UsuarioId");
            SQL.AppendLine("    ,@CodigoErro");
            SQL.AppendLine("    ,@MensagemErro)");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@AutorizacaoNotaFiscalId",SqlDbType.Int) { Value = model.AutorizacaoNotaFiscalId },
                new SqlParameter("@UsuarioId",SqlDbType.Int) { Value = model.UsuarioId },
                new SqlParameter("@CodigoErro",SqlDbType.VarChar) {Value = model.CodigoErro },
                new SqlParameter("@MensagemErro",SqlDbType.VarChar) { Value = model.MensagemErro }
            };

            return DataBase.Execute(SQL, sqlParameters);
        }
    }
}