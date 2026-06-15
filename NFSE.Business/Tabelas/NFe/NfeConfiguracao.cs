using NFSE.Infra.Data;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeConfiguracao
    {
        public string GetRemoteServer(bool isNFSeNacional = false)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT Server");
            SQL.AppendLine("  FROM dbo.tb_dep_nfe_configuracoes");

            using (var dtConfiguracoes = DataBase.Select(SQL))
            {
                return dtConfiguracoes.Rows[0]["Server"].ToString() + (isNFSeNacional ? "n" : string.Empty);
            }
        }

        public void SetContextInfo(int usuarioId)
        {
            DataBase.SetContextInfo(usuarioId);
        }
    }
}