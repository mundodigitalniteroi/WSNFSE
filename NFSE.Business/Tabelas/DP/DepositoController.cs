using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class DepositoController
    {
        public List<DepositoEntity> Listar(int depositoId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_depositos.id_deposito AS DepositoId");

            SQL.AppendLine("      ,tb_dep_depositos.id_empresa AS EmpresaId");

            SQL.AppendLine("      ,tb_dep_depositos.id_cep AS CepId");

            SQL.AppendLine("      ,tb_dep_depositos.id_tipo_logradouro AS TipoLogradouroId");

            SQL.AppendLine("      ,tb_dep_depositos.id_bairro AS BairroId");

            SQL.AppendLine("      ,tb_dep_depositos.id_sistema_externo AS SistemaExternoId");

            SQL.AppendLine("      ,tb_dep_depositos.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_depositos.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_depositos.descricao AS Descricao");

            SQL.AppendLine("      ,tb_dep_depositos.logradouro AS Logradouro");

            SQL.AppendLine("      ,tb_dep_depositos.numero AS Numero");

            SQL.AppendLine("      ,tb_dep_depositos.complemento AS Complemento");

            SQL.AppendLine("      ,tb_dep_depositos.email_nfe AS EmailNfe");

            SQL.AppendLine("      ,tb_dep_depositos.grv_minimo_fotos_exigidas AS GrvMinimoFotosExigidas");

            SQL.AppendLine("      ,tb_dep_depositos.grv_limite_minimo_datahora_guarda AS GrvLimiteMinimoDatahoraGuarda");

            SQL.AppendLine("      ,tb_dep_depositos.latitude AS Latitude");

            SQL.AppendLine("      ,tb_dep_depositos.longitude AS Longitude");

            SQL.AppendLine("      ,tb_dep_depositos.endereco_mob AS EnderecoMob");

            SQL.AppendLine("      ,tb_dep_depositos.telefone_mob AS TelefoneMob");

            SQL.AppendLine("      ,tb_dep_depositos.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_depositos.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_depositos.flag_endereco_cadastro_manual AS FlagEnderecoCadastroManual");

            SQL.AppendLine("      ,tb_dep_depositos.flag_ativo AS FlagAtivo");

            SQL.AppendLine("      ,tb_dep_depositos.flag_virtual AS FlagVirtual");

            SQL.AppendLine("  FROM dbo.tb_dep_depositos");

            SQL.Append(" WHERE tb_dep_depositos.id_deposito = ").Append(depositoId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<DepositoEntity>(dataTable);
            }
        }

        public DepositoEntity Selecionar(int depositoId)
        {
            var list = Listar(depositoId);

            return list?.FirstOrDefault();
        }
    }
}