using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class NfeMensagemController
    {
        public List<NfeMensagemEntity> Listar(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_mensagens.NfeMensagemID AS NfemensagemId");

            SQL.AppendLine("      ,tb_dep_nfe_mensagens.NfeID AS NfeId");

            SQL.AppendLine("      ,tb_dep_nfe_mensagens.Mensagem AS Mensagem");

            SQL.AppendLine("      ,tb_dep_nfe_mensagens.Tipo AS Tipo");

            SQL.AppendLine("      ,tb_dep_nfe_mensagens.DataCadastro AS Datacadastro");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_mensagens");

            SQL.AppendLine("   AND tb_dep_NfeMensagem.NfeId = " + nfeId);

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeMensagemEntity>(dataTable);
            }
        }

        public NfeMensagemEntity Selecionar(int nfeId)
        {
            var list = Listar(nfeId);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeMensagemEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_mensagens");

            SQL.AppendLine("      (NfeID");
            SQL.AppendLine("      ,Mensagem");
            SQL.AppendLine("      ,Tipo)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      (" + model.NfeId);
            SQL.AppendLine("      ,'" + model.Mensagem + "'");
            SQL.AppendLine("      ,'" + model.Tipo + "'");

            return DataBase.ExecuteScalar(SQL);
        }
    }
}