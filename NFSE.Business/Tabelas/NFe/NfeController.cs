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
        public List<NfeEntity> ListarPorIdentificadorNota(string identificadorNota)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe.NfeId");

            SQL.AppendLine("      ,tb_dep_nfe.GrvId");

            SQL.AppendLine("      ,tb_dep_nfe.IdentificadorNota");

            SQL.AppendLine("      ,tb_dep_nfe.NfeComplementarId");

            SQL.AppendLine("      ,tb_dep_nfe.UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_nfe.Cnpj");

            SQL.AppendLine("      ,tb_dep_nfe.Numero");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoVerificacao");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoRetorno");

            SQL.AppendLine("      ,tb_dep_nfe.Url");

            SQL.AppendLine("      ,tb_dep_nfe.Status");

            SQL.AppendLine("      ,tb_dep_nfe.StatusNfe");

            SQL.AppendLine("      ,tb_dep_nfe.DataEmissao");

            SQL.AppendLine("      ,tb_dep_nfe.DataCadastro");

            SQL.AppendLine("      ,tb_dep_nfe.DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe");

            SQL.AppendLine(" WHERE IdentificadorNota = @IdentificadorNota");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@IdentificadorNota",SqlDbType.VarChar) { Value = identificadorNota }
            };

            using (var dataTable = DataBase.Select(SQL, sqlParameters))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public List<NfeEntity> Listar(NfeEntity model, bool selecionarNotaFiscalCancelada = false)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe.NfeId");

            SQL.AppendLine("      ,tb_dep_nfe.GrvId");

            SQL.AppendLine("      ,tb_dep_nfe.IdentificadorNota");

            SQL.AppendLine("      ,tb_dep_nfe.NfeComplementarId");

            SQL.AppendLine("      ,tb_dep_nfe.UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_nfe.Cnpj");

            SQL.AppendLine("      ,tb_dep_nfe.Numero");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoVerificacao");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoRetorno");

            SQL.AppendLine("      ,tb_dep_nfe.Url");

            SQL.AppendLine("      ,tb_dep_nfe.Status");

            SQL.AppendLine("      ,tb_dep_nfe.StatusNfe");

            SQL.AppendLine("      ,tb_dep_nfe.DataEmissao");

            SQL.AppendLine("      ,tb_dep_nfe.DataCadastro");

            SQL.AppendLine("      ,tb_dep_nfe.DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe");

            SQL.AppendLine(" WHERE 1 = 1");

            if (!string.IsNullOrWhiteSpace(model.IdentificadorNota))
            {
                SQL.Append("   AND IdentificadorNota = '").Append(model.IdentificadorNota).AppendLine("'");
            }
            else if (model.GrvId > 0)
            {
                SQL.Append("   AND GrvId = ").Append(model.GrvId).AppendLine();
            }

            if (model.NfeId > 0)
            {
                SQL.Append("   AND NfeId = ").Append(model.NfeId).AppendLine();
            }

            if (!selecionarNotaFiscalCancelada)
            {
                SQL.AppendLine("   AND Status NOT IN ('N','E','I')");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public NfeEntity Selecionar(NfeEntity model, bool selecionarNotaFiscalCancelada = false)
        {
            var list = Listar(model, selecionarNotaFiscalCancelada);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe");

            SQL.AppendLine("      (GrvID");
            SQL.AppendLine("      ,UsuarioCadastroID");
            SQL.AppendLine("      ,Cnpj");
            SQL.AppendLine("      ,IdentificadorNota");
            SQL.AppendLine("      ,NfeComplementarId");
            SQL.AppendLine("      ,Status)");

            SQL.AppendLine("VALUES");

            SQL.Append("      (").Append(model.GrvId).AppendLine();
            SQL.Append("      ,").Append(model.UsuarioCadastroId).AppendLine();
            SQL.Append("      ,'").Append(model.Cnpj).AppendLine("'");
            SQL.Append("      ,'").Append(model.IdentificadorNota).AppendLine("'");

            if (model.NfeComplementarId == null)
            {
                SQL.AppendLine("      ,NULL");
                SQL.AppendLine("      ,'A')");
            }
            else
            {
                SQL.Append("      ,").Append(model.NfeComplementarId).AppendLine();
                SQL.AppendLine("      ,'R')");
            }

            try
            {
                return DataBase.ExecuteScopeIdentity(SQL);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ": " + SQL.ToString());
            }
        }

        public int Atualizar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.Append("   SET Status = '").Append(model.Status).AppendLine("'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.Append("       ,CodigoRetorno = ").Append(model.CodigoRetorno).AppendLine();
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.Append(" WHERE NfeID = ").Append(model.NfeId).AppendLine();

            return DataBase.Execute(SQL);
        }

        public int AtualizarRetornoNotaFiscal(NfeEntity nfe, RetornoNotaFiscalEntity retornoNotaFiscal)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.Append("   SET Referencia = '").Append(retornoNotaFiscal.@ref.Trim()).AppendLine("'");

            if (retornoNotaFiscal.numero_rps != null)
            {
                SQL.Append("      ,Numero = '").Append(retornoNotaFiscal.numero_rps.Trim()).AppendLine("'");

                SQL.Append("      ,NumeroRps = '").Append(retornoNotaFiscal.numero_rps.Trim()).AppendLine("'");
            }

            if (retornoNotaFiscal.serie_rps != null)
            {
                SQL.Append("      ,SerieRps = '").Append(retornoNotaFiscal.serie_rps.Trim()).AppendLine("'");
            }

            SQL.Append("      ,StatusNfe = '").Append(retornoNotaFiscal.status.Trim()).AppendLine("'");

            SQL.Append("      ,NumeroNotaFiscal = '").Append(retornoNotaFiscal.numero.Trim()).AppendLine("'");

            SQL.Append("      ,CodigoVerificacao = '").Append(retornoNotaFiscal.codigo_verificacao.Trim()).AppendLine("'");

            SQL.Append("      ,DataEmissao = '").Append(retornoNotaFiscal.data_emissao.ToString("yyyyMMdd HH:mm:ss")).AppendLine("'");

            SQL.Append("      ,Url = '").Append(retornoNotaFiscal.url.Trim()).AppendLine("'");

            SQL.Append("      ,CaminhoXmlNotaFiscal = '").Append(retornoNotaFiscal.caminho_xml_nota_fiscal.Trim()).AppendLine("'");

            SQL.Append("      ,Status = '").Append(nfe.Status).AppendLine("'");

            SQL.Append(" WHERE NfeID = ").Append(nfe.NfeId).AppendLine();

            return DataBase.Execute(SQL);
        }

        public int AguardandoProcessamento(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.Append("   SET Status = '").Append(model.Status).AppendLine("'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.Append("      ,CodigoRetorno = ").Append(model.CodigoRetorno).AppendLine();
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.Append(" WHERE NfeID = ").Append(model.NfeId).AppendLine();

            return DataBase.Execute(SQL);
        }

        public NfeEntity ConsultarNotaFiscal(int grvId, int usuarioId, string identificadorNota, Acao acao)
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

        public NfeEntity ConsultarNotaFiscal(string identificadorNota)
        {
            List<NfeEntity> nfe;

            try
            {
                if ((nfe = new NfeController().ListarPorIdentificadorNota(identificadorNota)) == null)
                {
                    throw new Exception("Nota Fiscal não encontrada no cadastro do Depósito Público (" + identificadorNota + ")");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            return nfe.FirstOrDefault();
        }

        public void Excluir(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("DELETE FROM dbo.tb_dep_nfe");

            SQL.Append(" WHERE NfeId = ").Append(nfeId).AppendLine();

            DataBase.Execute(SQL);
        }
    }
}