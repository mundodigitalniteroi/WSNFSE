using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
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

        public List<NfeEntity> Listar(int grvId, bool selecionarNotaFiscalCancelada = false)
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

            if (!selecionarNotaFiscalCancelada)
            {
                SQL.AppendLine("   AND Status != 'N'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public NfeEntity Selecionar(int grvId, bool selecionarNotaFiscalCancelada = false)
        {
            var list = Listar(grvId, selecionarNotaFiscalCancelada);

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

            return DataBase.ExecuteScopeIdentity(SQL);
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

        public int AtualizarRetornoNotaFiscal(int nfeId, RetornoNotaFiscalEntity retornoNotaFiscal)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.AppendLine("   SET Status = 'P'");

            SQL.AppendLine("      ,Numero = '" + retornoNotaFiscal.numero_rps + "'");

            SQL.AppendLine("      ,CodigoVerificacao = '" + retornoNotaFiscal.codigo_verificacao.Trim() + "'");

            SQL.AppendLine("      ,DataEmissao = '" + retornoNotaFiscal.data_emissao.ToString("yyyyMMdd HH:mm:ss") + "'");

            SQL.AppendLine("      ,StatusNfe = '" + retornoNotaFiscal.status + "'");

            SQL.AppendLine("      ,Url = '" + retornoNotaFiscal.url + "'");

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE NfeID = " + nfeId);

            return DataBase.Execute(SQL);
        }

        public int AguardandoProcessamento(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.AppendLine("   SET Status = 'A'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.AppendLine("      ,CodigoRetorno = " + model.CodigoRetorno);
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE NfeID = " + model.NfeId);
            SQL.AppendLine("   AND Status = 'C'");

            return DataBase.Execute(SQL);
        }

        public NfeEntity ConsultarNotaFiscal(int grvId, int usuarioId, int identificadorNota, Acao acao)
        {
            List<NfeEntity> nfe;

            try
            {
                if ((nfe = new NfeController().ListarPorIdentificadorNota(identificadorNota)) == null)
                {
                    new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Nota Fiscal não encontrada no cadastro do Depósito Público");

                    throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público (" + identificadorNota + ")");
                }
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, identificadorNota, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar a Nota Fiscal: " + ex.Message);

                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            return nfe.FirstOrDefault();
        }
    }
}