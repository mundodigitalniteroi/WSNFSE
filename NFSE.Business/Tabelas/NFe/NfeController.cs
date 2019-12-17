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
                new SqlParameter("@IdentificadorNota",SqlDbType.Int) { Value = identificadorNota }
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

            if (model.IdentificadorNota > 0)
            {
                SQL.AppendLine("   AND IdentificadorNota = " + model.IdentificadorNota);
            }
            else
            {
                SQL.AppendLine("   AND GrvId = " + model.GrvId);
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

            SQL.AppendLine("      (" + model.GrvId);
            SQL.AppendLine("      ," + model.UsuarioCadastroId);
            SQL.AppendLine("      ,'" + model.Cnpj + "'");
            SQL.AppendLine("      ," + model.IdentificadorNota);

            if (model.NfeComplementarId == null)
            {
                SQL.AppendLine("      ,NULL");
                SQL.AppendLine("      ,'A')");
            }
            else
            {
                SQL.AppendLine("      ," + model.NfeComplementarId);
                SQL.AppendLine("      ,'R')");
            }

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

        public int AtualizarRetornoNotaFiscal(NfeEntity nfe, RetornoNotaFiscalEntity retornoNotaFiscal)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.AppendLine("   SET Referencia = '" + retornoNotaFiscal.@ref.Trim() + "'");

            if (retornoNotaFiscal.numero_rps != null)
            {
                SQL.AppendLine("      ,Numero = '" + retornoNotaFiscal.numero_rps.Trim() + "'");

                SQL.AppendLine("      ,NumeroRps = '" + retornoNotaFiscal.numero_rps.Trim() + "'");
            }

            if (retornoNotaFiscal.serie_rps != null)
            {
                SQL.AppendLine("      ,SerieRps = '" + retornoNotaFiscal.serie_rps.Trim() + "'");
            }

            SQL.AppendLine("      ,StatusNfe = '" + retornoNotaFiscal.status.Trim() + "'");

            SQL.AppendLine("      ,NumeroNotaFiscal = '" + retornoNotaFiscal.numero.Trim() + "'");

            SQL.AppendLine("      ,CodigoVerificacao = '" + retornoNotaFiscal.codigo_verificacao.Trim() + "'");

            SQL.AppendLine("      ,DataEmissao = '" + retornoNotaFiscal.data_emissao.ToString("yyyyMMdd HH:mm:ss") + "'");

            SQL.AppendLine("      ,Url = '" + retornoNotaFiscal.url.Trim() + "'");

            SQL.AppendLine("      ,CaminhoXmlNotaFiscal = '" + retornoNotaFiscal.caminho_xml_nota_fiscal.Trim() + "'");

            SQL.AppendLine(" WHERE NfeID = " + nfe.NfeId);

            return DataBase.Execute(SQL);
        }

        public int AguardandoProcessamento(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe");

            SQL.AppendLine("   SET Status = '" + model.Status + "'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.AppendLine("      ,CodigoRetorno = " + model.CodigoRetorno);
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE NfeID = " + model.NfeId);

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