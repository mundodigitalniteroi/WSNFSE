using NFSE.Infra.Data;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class Server
    {
        public string GetRemoteServer()
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT Server");
            SQL.AppendLine("  FROM " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_configuracoes");

            using (var dtConfiguracoes = DataBase.Select(SQL))
            {
                return dtConfiguracoes.Rows[0]["Server"].ToString();
            }
        }

        public void SetContextInfo(int usuarioId)
        {
            DataBase.SetContextInfo(usuarioId);
        }
    }
}