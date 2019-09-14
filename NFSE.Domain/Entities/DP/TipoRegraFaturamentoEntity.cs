namespace NFSE.Domain.Entities.DP
{
    public class TipoRegraFaturamentoEntity
    {
        public short FaturamentoRegraTipoId { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public char FlagPossuiValor { get; set; }

        public char FlagAtivo { get; set; }
    }
}