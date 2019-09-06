using NFSE.Infra.Data;
using System.Text;

namespace Negocio.Tabelas
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
    }
}