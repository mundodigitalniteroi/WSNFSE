using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class ClienteDepositoController
    {
        public List<ClienteDepositoEntity> Listar(ClienteDepositoEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_clientes_depositos.id_cliente_deposito AS ClienteDepositoId");

            SQL.AppendLine("      ,tb_dep_clientes_depositos.id_cliente AS ClienteId");

            SQL.AppendLine("      ,tb_dep_clientes_depositos.id_deposito AS DepositoId");

            SQL.AppendLine("      ,tb_dep_clientes_depositos.id_empresa AS EmpresaId");

            SQL.AppendLine("      ,tb_dep_clientes_depositos.AliquotaIss");

            SQL.AppendLine("      ,tb_dep_clientes_depositos.FlagValorIssIgualProdutoBaseCalculoAliquota");

            SQL.AppendLine("  FROM dbo.tb_dep_clientes_depositos");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.ClienteDepositoId > 0)
            {
                SQL.Append("   AND tb_dep_clientes_depositos.id_cliente_deposito = ").Append(model.ClienteDepositoId).AppendLine();
            }

            if (model.ClienteId > 0)
            {
                SQL.Append("   AND tb_dep_clientes_depositos.id_cliente = ").Append(model.ClienteId).AppendLine();
            }

            if (model.DepositoId > 0)
            {
                SQL.Append("   AND tb_dep_clientes_depositos.id_deposito = ").Append(model.DepositoId).AppendLine();
            }

            if (model.EmpresaId > 0)
            {
                SQL.Append("   AND tb_dep_clientes_depositos.id_empresa = ").Append(model.EmpresaId).AppendLine();
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<ClienteDepositoEntity>(dataTable);
            }
        }

        public ClienteDepositoEntity Selecionar(ClienteDepositoEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}