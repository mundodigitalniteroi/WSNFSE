using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Data;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class PrestadorController
    {
        public DataTable Consultar(string cnpj)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT a.id_nfse_prestador");
            SQL.AppendLine("      ,a.prestador_cnpj");
            SQL.AppendLine("      ,a.prestador_nome");
            SQL.AppendLine("      ,a.prestador_inscricao_municipal");
            SQL.AppendLine("      ,a.prestador_codigo_municipio_ibge");
            SQL.AppendLine("      ,a.prestador_chave");
            SQL.AppendLine("      ,a.prestador_data_cadastro");
            SQL.AppendLine("  FROM " + DataBase.GetNfeDatabase() + ".dbo.tb_nfse_prestador a");

            SQL.AppendLine(" WHERE a.prestador_cnpj = '" + cnpj + "'");

            return DataBase.Select(SQL);
        }

        public PrestadorServico ConsultarPrestadorServico(int grvId, int usuarioId, string cnpj, Acao acao, NfeEntity nfe, CapaAutorizacaoNfse capaAutorizacaoNfse = null)
        {
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            cnpj = cnpj.Replace("/", "").Replace(".", "").Replace("-", "");

            var prestadorAcesso = new PrestadorServico();

            try
            {
                prestadorAcesso.server = new NfeConfiguracao().GetRemoteServer();
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar os dados do Servidor: " + ex);

                throw new Exception("Ocorreu um erro ao consultar os dados do Servidor: " + ex);
            }

            try
            {
                using (var dtPrestador = new PrestadorController().Consultar(cnpj))
                {
                    if (dtPrestador == null)
                    {
                        new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Prestador de Serviços não configurado. CNPJ: " + cnpj);

                        throw new Exception("Prestador de Serviços não configurado. CNPJ: " + cnpj);
                    }

                    foreach (DataRow row in (InternalDataCollectionBase)dtPrestador.Rows)
                    {
                        prestadorAcesso.id_nfse_prestador = row["id_nfse_prestador"].ToString();
                        prestadorAcesso.prestador_cnpj = row["prestador_cnpj"].ToString();
                        prestadorAcesso.prestador_nome = row["prestador_nome"].ToString();
                        prestadorAcesso.prestador_inscricao_municipal = row["prestador_inscricao_municipal"].ToString();
                        prestadorAcesso.prestador_codigo_municipio_ibge = row["prestador_codigo_municipio_ibge"].ToString();
                        prestadorAcesso.prestador_chave = row["prestador_chave"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Ocorreu um erro ao consultar o Prestador de Serviços: " + ex.Message);

                throw new Exception("Ocorreu um erro ao consultar o Prestador de Serviços: " + ex.Message);
            }

            if (string.IsNullOrEmpty(prestadorAcesso.prestador_chave))
            {
                new NfeWsErroController().CadastrarErroGenerico(grvId, usuarioId, nfe.IdentificadorNota, OrigemErro.MobLink, acao, "Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);

                throw new Exception("Prestador de Serviços sem chave configurada (token). CNPJ: " + cnpj);
            }

            return prestadorAcesso;
        }
    }
}