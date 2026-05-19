using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System;

namespace NFSE.Business.Tabelas.Global
{
    public class MunicipioController
    {
        public List<MunicipioEntity> Listar(string uf)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_glo_loc_municipios.id_municipio AS MunicipioId");

            SQL.AppendLine("      ,tb_glo_loc_municipios.uf AS Uf");

            SQL.AppendLine("      ,tb_glo_loc_municipios.nome AS Nome");

            SQL.AppendLine("      ,tb_glo_loc_municipios.nome_ptbr AS NomePtbr");

            SQL.AppendLine("      ,tb_glo_loc_municipios.codigo_municipio AS CodigoMunicipio");

            SQL.AppendLine("      ,tb_glo_loc_municipios.codigo_municipio_ibge AS CodigoMunicipioIbge");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_municipios");

            SQL.Append(" WHERE tb_glo_loc_municipios.uf = '").Append(uf).AppendLine("'");

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<MunicipioEntity>(dataTable);
            }
        }

        public MunicipioEntity Selecionar(string uf)
        {
            var list = Listar(uf);

            return list?.FirstOrDefault();
        }

        public string SelecionarPrimeiroCodigoIbge(string uf, string municipio)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT MIN(codigo_municipio_ibge) AS CodigoMunicipioIbge");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_loc_municipios");

            SQL.Append(" WHERE uf = '").Append(uf).AppendLine("'");
            SQL.Append(" AND lower(nome) = '").Append(RemoverAcentos(municipio.ToLower())).AppendLine("'");
            SQL.AppendLine("   AND codigo_municipio_ibge IS NOT NULL");

            using (var result = DataBase.Select(SQL))
            {
                if (result == null)
                {
                    return string.Empty;
                }

                return result.Rows[0]["CodigoMunicipioIbge"].ToString();
            }
        }

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}