using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeFaturamentoComposicaoController
    {
        public List<NfeFaturamentoComposicaoEntity> Listar(int nfeId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe_faturamento_composicao.NfeFaturamentoComposicaoId");

            SQL.AppendLine("      ,tb_dep_nfe_faturamento_composicao.NfeId");

            SQL.AppendLine("      ,tb_dep_nfe_faturamento_composicao.FaturamentoComposicaoId");

            SQL.AppendLine("  FROM dbo.tb_dep_nfe_faturamento_composicao");

            SQL.AppendLine(" WHERE tb_dep_nfe_faturamento_composicao.NfeId = " + nfeId);

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable != null ? DataTableUtil.DataTableToList<NfeFaturamentoComposicaoEntity>(dataTable) : null;
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

            //SQL.AppendLine("      (NfeId");
            //SQL.AppendLine("      ,FaturamentoComposicaoId)");

            SQL.AppendLine("VALUES");

            var lastItem = list.LastOrDefault();

            foreach (var item in list)
            {
                SQL.AppendLine("      (" + nfeId);
                SQL.Append    ("      ," + item.FaturamentoComposicaoId + ")");

                if (item != lastItem)
                {
                    SQL.AppendLine(",");
                }
            }

            DataBase.Execute(SQL);
        }
    }
}