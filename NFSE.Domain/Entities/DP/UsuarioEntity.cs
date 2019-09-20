using System;

namespace NFSE.Domain.Entities.DP
{
    public class UsuarioEntity
    {
        public int UsuarioId { get; set; }

        public int FuncionarioId { get; set; }

        public byte TipoOperadorId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Login { get; set; }

        public string Senha1 { get; set; }

        public string Senha2 { get; set; }

        public string Senha3 { get; set; }

        public string Senha4 { get; set; }

        public string Senha5 { get; set; }

        public string SenhaAndroId { get; set; }

        public string Email { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public DateTime DataCadastroSenha { get; set; }

        public DateTime? DataUltimoAcesso { get; set; }

        public char FlagPermissaoDesconto { get; set; }

        public char FlagPermissaoDataRetroativaFaturamento { get; set; }

        public char FlagReceberEmailErro { get; set; }

        public char FlagAtivo { get; set; }

        public char Dummy { get; set; }

        public int? PessoaId { get; set; }

        public string Matricula { get; set; }
    }
}