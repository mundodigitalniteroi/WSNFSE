using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeRegraEntity
    {
        public short NfeRegraId { get; set; }

        public short NfeRegraTipoId { get; set; }

        public int ClienteId { get; set; }

        public int DepositoId { get; set; }

        public int ClienteDepositoId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Valor { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public int Ativo { get; set; }

        public string RegraCodigo { get; set; }

        public string RegraDescricao { get; set; }

        public int RegraPossuiValor { get; set; }

        public int RegraAtivo { get; set; }

        public string Cliente { get; set; }

        public string Deposito { get; set; }

        public string UsuarioCadastro { get; set; }

        public string UsuarioAlteracao { get; set; }
    }
}