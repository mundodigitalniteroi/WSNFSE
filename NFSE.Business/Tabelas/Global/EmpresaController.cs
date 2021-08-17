using NFSE.Domain.Entities.Global;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.Global
{
    public class EmpresaController
    {
        public List<EmpresaEntity> Listar(EmpresaEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_glo_emp_empresas.id_empresa AS EmpresaId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_empresa_matriz AS EmpresaMatrizId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_empresa_classificacao AS EmpresaClassificacaoId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_cep AS CepId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_tipo_logradouro AS TipoLogradouroId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.cnpj AS Cnpj");

            SQL.AppendLine("      ,tb_glo_emp_empresas.nome AS Nome");

            SQL.AppendLine("      ,tb_glo_emp_empresas.nome_fantasia AS NomeFantasia");

            SQL.AppendLine("      ,tb_glo_emp_empresas.logradouro AS Logradouro");

            SQL.AppendLine("      ,tb_glo_emp_empresas.numero AS Numero");

            SQL.AppendLine("      ,tb_glo_emp_empresas.complemento AS Complemento");

            SQL.AppendLine("      ,tb_glo_emp_empresas.bairro AS Bairro");

            SQL.AppendLine("      ,tb_glo_emp_empresas.municipio AS Municipio");

            SQL.AppendLine("      ,tb_glo_emp_empresas.uf AS Uf");

            SQL.AppendLine("      ,tb_glo_emp_empresas.CodigoAlterdata AS CodigoAlterdata");

            SQL.AppendLine("      ,tb_glo_emp_empresas.codigo_sap AS CodigoSap");

            SQL.AppendLine("      ,tb_glo_emp_empresas.CnaeID AS CnaeId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.CnaeListaServicoID AS CnaeListaServicoId");

            SQL.AppendLine("      ,tb_glo_emp_empresas.inscricao_estadual AS InscricaoEstadual");

            SQL.AppendLine("      ,tb_glo_emp_empresas.inscricao_municipal AS InscricaoMunicipal");

            SQL.AppendLine("      ,tb_glo_emp_empresas.codigo_tributario_municipio AS CodigoTributarioMunicipio");

            SQL.AppendLine("      ,tb_glo_emp_empresas.Token AS Token");

            SQL.AppendLine("      ,tb_glo_emp_empresas.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_glo_emp_empresas.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_glo_emp_empresas.flag_iss_retido AS FlagIssRetido");

            SQL.AppendLine("      ,tb_glo_emp_empresas.flag_ativo AS FlagAtivo");

            SQL.AppendLine("      ,tb_glo_emp_empresas.optante_simples_nacional AS OptanteSimplesNacional");

            SQL.AppendLine("  FROM db_global.dbo.tb_glo_emp_empresas");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.EmpresaId > 0)
            {
                SQL.AppendLine("   AND tb_glo_emp_empresas.id_empresa = " + model.EmpresaId);
            }

            if (!string.IsNullOrWhiteSpace(model.Cnpj))
            {
                SQL.AppendLine("   AND tb_glo_emp_empresas.cnpj = '" + model.Cnpj + "'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<EmpresaEntity>(dataTable);
            }
        }

        public EmpresaEntity Selecionar(EmpresaEntity empresa)
        {
            var list = Listar(empresa);

            return list?.FirstOrDefault();
        }
    }
}