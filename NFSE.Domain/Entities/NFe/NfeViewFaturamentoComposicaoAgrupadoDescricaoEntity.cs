namespace NFSE.Domain.Entities.NFe
{
    public class NfeViewFaturamentoComposicaoAgrupadoDescricaoEntity
    {
        public string NumeroFormularioGrv { get; set; }

        public int GrvId { get; set; }

        public int AtendimentoId { get; set; }

        public int FaturamentoId { get; set; }

        public int CnaeId { get; set; }

        public string Cnae { get; set; }

        public int ListaServicoId { get; set; }

        public string Servico { get; set; }

        public string DescricaoConfiguracaoNfe { get; set; }

        public char FlagEnviarValorIss { get; set; }

        public char FlagEnviarInscricaoEstadual { get; set; }

        public decimal QuantidadeComposicao { get; set; }
        
        public decimal ValorTipoComposicao { get; set; }
        
        public decimal ValorCalculadoSemDesconto { get; set; }

        public char TipoDesconto { get; set; }
        
        public decimal QuantidadeDesconto { get; set; }
        
        public decimal ValorDesconto { get; set; }

        public decimal TotalComDesconto { get; set; }
    }
}