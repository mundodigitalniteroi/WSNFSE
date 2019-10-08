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

            SQL.AppendLine("  FROM dbo.tb_dep_clientes_depositos");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.ClienteDepositoId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_cliente_deposito = " + model.ClienteDepositoId);
            }

            if (model.ClienteId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_cliente = " + model.ClienteId);
            }

            if (model.DepositoId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_deposito = " + model.DepositoId);
            }

            if (model.EmpresaId > 0)
            {
                SQL.AppendLine("   AND tb_dep_clientes_depositos.id_empresa = " + model.EmpresaId);
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<ClienteDepositoEntity>(dataTable);
            }
        }

        public ClienteDepositoEntity Selecionar(ClienteDepositoEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}