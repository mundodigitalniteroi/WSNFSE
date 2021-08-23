using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class EnderecoCompletoController
    {
        public List<EnderecoCompletoEntity> Listar(EnderecoCompletoEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_glo_consultar_endereco_completo.id_cep AS CepId");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.id_municipio AS MunicipioId");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.id_bairro AS BairroId");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.id_tipo_logradouro AS TipoLogradouroId");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.CEP AS CEP");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.tipo_logradouro AS TipoLogradouro");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.logradouro AS Logradouro");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.bairro AS Bairro");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.bairro_ptbr AS BairroPtbr");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.municipio AS Municipio");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.municipio_ptbr AS MunicipioPtbr");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.codigo_municipio AS CodigoMunicipio");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.codigo_municipio_ibge AS CodigoMunicipioIbge");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.estado AS Estado");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.estado_ptbr AS EstadoPtbr");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.uf AS Uf");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.regiao AS Regiao");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.regiao_nome AS RegiaoNome");

            SQL.AppendLine("      ,vw_glo_consultar_endereco_completo.flag_sanitizado AS FlagSanitizado");

            SQL.AppendLine("  FROM db_global.dbo.vw_glo_consultar_endereco_completo");

            if (model.CepId > 0)
            {
                SQL.Append(" WHERE vw_glo_consultar_endereco_completo.id_cep = ").Append(model.CepId).AppendLine();
            }
            else
            {
                SQL.Append(" WHERE vw_glo_consultar_endereco_completo.cep = '").Append(model.Cep).AppendLine("'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<EnderecoCompletoEntity>(dataTable);
            }
        }

        public EnderecoCompletoEntity Selecionar(int cepId)
        {
            var list = Listar(new EnderecoCompletoEntity { CepId = cepId });

            return list?.FirstOrDefault();
        }

        public EnderecoCompletoEntity Selecionar(string cep)
        {
            var list = Listar(new EnderecoCompletoEntity { Cep = cep });

            return list?.FirstOrDefault();
        }
    }
}