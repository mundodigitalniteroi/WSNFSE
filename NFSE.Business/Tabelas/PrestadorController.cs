using NFSE.Domain.Entities;
using NFSE.Infra.Data;
using System.Data;
using System.Text;

namespace NFSE.Business.Tabelas
{
    public class PrestadorController
    {
        public DataTable Consultar(string cnpj, CapaAutorizacaoNfse capaAutorizacaoNfse = null)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT a.id_nfse_prestador");
            SQL.AppendLine("      ,a.prestador_cnpj");
            SQL.AppendLine("      ,a.prestador_nome");
            SQL.AppendLine("      ,a.prestador_inscricao_municipal");
            SQL.AppendLine("      ,a.prestador_codigo_municipio_ibge");
            SQL.AppendLine("      ,a.prestador_codigo_municipio_ibge");
            SQL.AppendLine("      ,a.prestador_chave");
            SQL.AppendLine("      ,a.prestador_data_cadastro");
            SQL.AppendLine("      ,b.item_lista_servico");
            SQL.AppendLine("      ,b.codigo_tributario_municipio");
            SQL.AppendLine("      ,b.codigo_cnae");
            SQL.AppendLine("  FROM " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_prestador a");
            SQL.AppendLine("  LEFT JOIN " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_parametro_municipio b");
            SQL.AppendLine("    ON b.codigo_ibge = a.prestador_codigo_municipio_ibge");

            if (capaAutorizacaoNfse != null)
            {
                SQL.AppendLine("   AND b.item_lista_servico = '" + capaAutorizacaoNfse.Autorizacao.servico.item_lista_servico + "'");
            }

            SQL.AppendLine(" WHERE a.prestador_cnpj = '" + cnpj + "'");

            if (capaAutorizacaoNfse != null)
            {
                SQL.AppendLine("   AND a.prestador_codigo_municipio_ibge = '" + capaAutorizacaoNfse.Autorizacao.prestador.codigo_municipio + "'");
            }

            return DataBase.Select(SQL);
        }
    }
}