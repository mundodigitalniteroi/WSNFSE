using System;

namespace NFSE.Domain.Entities.DP
{
    public class FaturamentoEntity
    {
        public int FaturamentoId { get; set; }

        public int AtendimentoId { get; set; }

        public byte? TipoMeioCobrancaId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string NumeroIdentificacao { get; set; }

        public decimal ValorFaturado { get; set; }

        public decimal? ValorPagamento { get; set; }

        public string HoraDiaria { get; set; }

        public short MaximoDiariasParaCobranca { get; set; }

        public short MaximoDiasVencimento { get; set; }

        public byte Sequencia { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public DateTime? DataCalculo { get; set; }

        public DateTime? DataRetroativa { get; set; }

        public DateTime DataVencimento { get; set; }

        public DateTime? DataPrazoRetiradaVeiculo { get; set; }

        public DateTime? DataPagamento { get; set; }

        public DateTime? DataEmissaoDocumento { get; set; }

        public DateTime? DataEmissaoNotaFiscal { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public char Status { get; set; }

        public char FlagUsarHoraDiaria { get; set; }

        public char FlagLimitacaoJudicial { get; set; }

        public char FlagClienteRealizaFaturamentoArrecadacao { get; set; }

        public char FlagCobrarDiariasDiasCorridos { get; set; }

        public char FlagPermissaoDataRetroativaFaturamento { get; set; }
    }
}