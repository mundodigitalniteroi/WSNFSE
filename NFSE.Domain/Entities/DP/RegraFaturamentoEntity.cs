using System;

namespace NFSE.Domain.Entities.DP
{
    public class RegraFaturamentoEntity
    {
        public short FaturamentoRegraId { get; set; }

        public short FaturamentoRegraTipoId { get; set; }

        public int? ClienteId { get; set; }

        public int? DepositoId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Valor { get; set; }

        public DateTime DataVigenciaInicial { get; set; }

        public DateTime? DataVigenciaFinal { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }
    }
}