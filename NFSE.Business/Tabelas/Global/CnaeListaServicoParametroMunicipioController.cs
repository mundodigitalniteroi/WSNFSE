using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class CnaeListaServicoParametroMunicipioController
    {
        public List<CnaeListaServicoParametroMunicipioEntity> Listar(CnaeListaServicoParametroMunicipioEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_gov_cnae_lista_servico_parametro_municipio.CnaeListaServicoId AS CnaeListaServicoId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CnaeId AS CnaeId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.ListaServicoId AS ListaServicoId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.MunicipioId AS MunicipioId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.ParametroMunicipioId AS ParametroMunicipioId");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CnaeCodigo AS CnaeCodigo");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CnaeCodigoFormatado AS CnaeCodigoFormatado");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CnaeDescricao AS CnaeDescricao");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.ListaServico AS ListaServico");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.ListaServicoDescricao AS ListaServicoDescricao");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.AliquotaIss AS AliquotaIss");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.Uf AS Uf");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.Municipio AS Municipio");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CodigoMunicipioIbge AS CodigoMunicipioIbge");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CodigoTributarioMunicipio AS CodigoTributarioMunicipio");

            SQL.AppendLine("      ,vw_gov_cnae_lista_servico_parametro_municipio.CodigoTributacaoNacionalIss AS CodigoTributacaoNacionalIss");

            SQL.AppendLine("  FROM db_global.dbo.vw_gov_cnae_lista_servico_parametro_municipio");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.CnaeId > 0)
            {
                SQL.Append("   AND vw_gov_cnae_lista_servico_parametro_municipio.CnaeId = ").Append(model.CnaeId).AppendLine();
            }

            if (model.ListaServicoId > 0)
            {
                SQL.Append("   AND vw_gov_cnae_lista_servico_parametro_municipio.ListaServicoId = ").Append(model.ListaServicoId).AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(model.CodigoMunicipioIbge))
            {
                SQL.Append("   AND vw_gov_cnae_lista_servico_parametro_municipio.CodigoMunicipioIbge = '").Append(model.CodigoMunicipioIbge).AppendLine("'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<CnaeListaServicoParametroMunicipioEntity>(dataTable);
            }
        }

        public CnaeListaServicoParametroMunicipioEntity Selecionar(CnaeListaServicoParametroMunicipioEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}