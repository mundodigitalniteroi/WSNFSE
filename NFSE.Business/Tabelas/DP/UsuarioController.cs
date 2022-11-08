using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class UsuarioController
    {
        public List<UsuarioEntity> Listar(int usuarioId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_usuarios.id_usuario AS UsuarioId");

            SQL.AppendLine("      ,tb_dep_usuarios.id_funcionario AS FuncionarioId");

            SQL.AppendLine("      ,tb_dep_usuarios.id_tipo_operador AS TipoOperadorId");

            SQL.AppendLine("      ,tb_dep_usuarios.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_usuarios.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_usuarios.login AS Login");

            SQL.AppendLine("      ,tb_dep_usuarios.senha1 AS Senha1");

            SQL.AppendLine("      ,tb_dep_usuarios.senha2 AS Senha2");

            SQL.AppendLine("      ,tb_dep_usuarios.senha3 AS Senha3");

            SQL.AppendLine("      ,tb_dep_usuarios.senha4 AS Senha4");

            SQL.AppendLine("      ,tb_dep_usuarios.senha5 AS Senha5");

            SQL.AppendLine("      ,tb_dep_usuarios.senha_android AS SenhaAndroId");

            SQL.AppendLine("      ,tb_dep_usuarios.email AS Email");

            SQL.AppendLine("      ,tb_dep_usuarios.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_usuarios.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_usuarios.data_cadastro_senha AS DataCadastroSenha");

            SQL.AppendLine("      ,tb_dep_usuarios.data_ultimo_acesso AS DataUltimoAcesso");

            SQL.AppendLine("      ,tb_dep_usuarios.flag_permissao_desconto AS FlagPermissaoDesconto");

            SQL.AppendLine("      ,tb_dep_usuarios.flag_permissao_data_retroativa_faturamento AS FlagPermissaoDataRetroativaFaturamento");

            SQL.AppendLine("      ,tb_dep_usuarios.flag_receber_email_erro AS FlagReceberEmailErro");

            SQL.AppendLine("      ,tb_dep_usuarios.flag_ativo AS FlagAtivo");

            SQL.AppendLine("      ,tb_dep_usuarios.dummy AS Dummy");

            SQL.AppendLine("      ,tb_dep_usuarios.PessoaID AS PessoaId");

            SQL.AppendLine("      ,tb_dep_usuarios.matricula AS Matricula");

            SQL.AppendLine("  FROM dbo.tb_dep_usuarios");

            SQL.Append(" WHERE tb_dep_usuarios.id_usuario = ").Append(usuarioId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<UsuarioEntity>(dataTable);
            }
        }

        public UsuarioEntity Selecionar(int usuarioId)
        {
            var list = Listar(usuarioId);

            return list?.FirstOrDefault();
        }
    }
}