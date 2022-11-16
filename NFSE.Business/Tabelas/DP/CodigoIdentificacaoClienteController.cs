using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public abstract class CodigoIdentificacaoClienteController
    {
        public static List<CodigoIdentificacaoClienteEntity> Listar(int grvId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT id_cliente_codigo_identificacao AS CodigoIdentificacaoClienteId");

            SQL.AppendLine("      ,id_grv AS GrvId");

            SQL.AppendLine("      ,id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,codigo_identificacao AS CodigoIdentificacao");

            SQL.AppendLine("      ,data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,data_alteracao AS DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_dep_grv_clientes_codigo_identificacao");

            SQL.Append(" WHERE id_grv = ").Append(grvId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<CodigoIdentificacaoClienteEntity>(dataTable);
            }
        }

        public static CodigoIdentificacaoClienteEntity Selecionar(int clienteId)
        {
            var list = Listar(clienteId);

            return list?.First();
        }
    }
}