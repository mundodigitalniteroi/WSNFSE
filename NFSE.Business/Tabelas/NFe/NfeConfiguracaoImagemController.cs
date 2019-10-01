using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeConfiguracaoImagemController
    {
        public List<NfeConfiguracaoImagemEntity> Listar(NfeConfiguracaoImagemEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_configuracao_imagem.ConfiguracaoImagemId");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.ClienteDepositoId");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.UsuarioCadastroId");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.UsuarioAlteracaoId");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.ValueX");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.ValueY");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.Width");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.Height");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.DataCadastro");
            SQL.AppendLine("      ,tb_dep_nfe_configuracao_imagem.DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_configuracao_imagem");

            SQL.AppendLine("  JOIN dbo.tb_dep_clientes_depositos");
            SQL.AppendLine("    ON tb_dep_clientes_depositos.id_cliente_deposito = tb_dep_nfe_configuracao_imagem.ClienteDepositoId");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.ConfiguracaoImagemId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.ConfiguracaoImagemId = " + model.ConfiguracaoImagemId);
            }
            else if (model.ClienteDepositoId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.ClienteDepositoId = " + model.ClienteDepositoId);
            }

            if (model.ClienteId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_cliente = " + model.ClienteId);
            }

            if (model.DepositoId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_deposito = " + model.DepositoId);
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeConfiguracaoImagemEntity>(dataTable);
            }
        }

        public NfeConfiguracaoImagemEntity Selecionar(NfeConfiguracaoImagemEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }

        #region Cadastrar

        public int Cadastrar(NfeConfiguracaoImagemEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_configuracao_imagem");

            SQL.AppendLine("      (ClienteDepositoId");
            SQL.AppendLine("      ,UsuarioCadastroId");
            SQL.AppendLine("      ,ValueX");
            SQL.AppendLine("      ,ValueY");
            SQL.AppendLine("      ,Width");
            SQL.AppendLine("      ,Height)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      (" + model.ClienteDepositoId);
            SQL.AppendLine("      ,1");
            SQL.AppendLine("      ," + model.ValueX);
            SQL.AppendLine("      ," + model.ValueY);
            SQL.AppendLine("      ," + model.Width);
            SQL.AppendLine("      ," + model.Height + ")");

            return DataBase.ExecuteScopeIdentity(SQL);
        }
        #endregion Cadastrar


        #region Atualizar
        public int Atualizar(NfeConfiguracaoImagemEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_dep_nfe_configuracao_imagem");

            SQL.AppendLine("   SET UsuarioAlteracaoId = 1");
            SQL.AppendLine("      ,ValueX = " + model.ValueX);
            SQL.AppendLine("      ,ValueY = " + model.ValueY);
            SQL.AppendLine("      ,Width = " + model.Width);
            SQL.AppendLine("      ,Height = " + model.Height);
            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE ConfiguracaoImagemId = " + model.ConfiguracaoImagemId);

            return DataBase.Execute(SQL);
        }
        #endregion Atualizar
    }
}