using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeFaturamentoComposicaoController
    {
        public List<NfeFaturamentoComposicaoEntity> Listar(int nfeId, int faturamentoComposicaoId = 0)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_faturamento_composicao.NfeFaturamentoComposicaoId");

            SQL.AppendLine("      ,tb_dep_nfe_faturamento_composicao.NfeId");

            SQL.AppendLine("      ,tb_dep_nfe_faturamento_composicao.FaturamentoComposicaoId");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_faturamento_composicao");

            if (nfeId > 0)
            {
                SQL.Append(" WHERE tb_dep_nfe_faturamento_composicao.NfeId = ").Append(nfeId).AppendLine();
            }
            else if (faturamentoComposicaoId > 0)
            {
                SQL.Append(" WHERE tb_dep_nfe_faturamento_composicao.FaturamentoComposicaoId = ").Append(faturamentoComposicaoId).AppendLine();
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfeFaturamentoComposicaoEntity>(dataTable);
            }
        }

        public NfeFaturamentoComposicaoEntity Selecionar(int grvId)
        {
            var list = Listar(grvId);

            return list?.FirstOrDefault();
        }

        public void Cadastrar(int nfeId, List<NfeViewFaturamentoComposicaoEntity> list)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_dep_nfe_faturamento_composicao");

            SQL.AppendLine("      (NfeId");
            SQL.AppendLine("      ,FaturamentoComposicaoId)");

            SQL.AppendLine("VALUES");

            var lastItem = list.Last();

            foreach (var item in list)
            {
                SQL.Append("      (").Append(nfeId).AppendLine();
                SQL.Append("      ,").Append(item.FaturamentoComposicaoId).Append(')');

                if (item != lastItem)
                {
                    SQL.AppendLine(",");
                }
            }

            DataBase.Execute(SQL);
        }

        public void Excluir(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("DELETE FROM dbo.tb_dep_nfe_faturamento_composicao");

            SQL.Append(" WHERE NfeId = ").Append(nfeId).AppendLine();

            DataBase.Execute(SQL);
        }
    }
}