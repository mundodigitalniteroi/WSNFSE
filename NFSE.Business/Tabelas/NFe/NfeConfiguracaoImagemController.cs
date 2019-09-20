using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeConfiguracaoImagemController
    {
        public List<NfeConfiguracaoImagemEntity> Listar(int clienteId, int depositoId)
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

            SQL.AppendLine(" WHERE tb_dep_clientes_depositos.id_cliente = " + clienteId);
            SQL.AppendLine("   AND tb_dep_clientes_depositos.id_deposito = " + depositoId);

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeConfiguracaoImagemEntity>(dataTable);
            }
        }

        public NfeConfiguracaoImagemEntity Selecionar(int clienteId, int depositoId)
        {
            var list = Listar(clienteId, depositoId);

            return list?.FirstOrDefault();
        }
    }
}