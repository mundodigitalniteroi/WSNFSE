using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;

namespace NFSE.Business.Tabelas.NFe
{
    public abstract class NfeExcluirController
    {
        public static bool Excluir(int identificadorNota)
        {
            List<NfeEntity> nfes = new NfeController().ListarPorIdentificadorNota(identificadorNota);

            if (nfes == null)
            {
                return false;
            }

            var nfe = nfes.First();

            DataBase.Execute("DELETE FROM tb_dep_nfe_faturamento_composicao WHERE NfeID = " + nfe.NfeId);

            DataBase.Execute("DELETE FROM tb_dep_nfe_imagens WHERE NfeID = " + nfe.NfeId);

            DataBase.Execute("DELETE FROM tb_dep_nfe_mensagens WHERE NfeID = " + nfe.NfeId);

            DataBase.Execute("DELETE FROM tb_dep_nfe_nota_fiscal WHERE NfeID = " + nfe.NfeId);

            DataBase.Execute("DELETE FROM tb_dep_nfe_retorno_solicitacao WHERE NfeID = " + nfe.NfeId);

            DataBase.Execute($"DELETE FROM tb_dep_nfe_ws_erros WHERE GrvId = {nfe.GrvId} AND IdentificadorNota = {nfe.IdentificadorNota}");

            DataBase.Execute("DELETE FROM tb_dep_nfe WHERE NfeID = " + nfe.NfeId);

            return true;
        }
    }
}