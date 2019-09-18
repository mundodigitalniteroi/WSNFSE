namespace NFSE.Domain.Entities.NFe
{
    public class Autorizacao
    {
        public string data_emissao { get; set; }

        public string natureza_operacao { get; set; }

        public string regime_especial_tributacao { get; set; }

        public string optante_simples_nacional { get; set; }

        public string incentivador_cultural { get; set; }

        public string tributacao_rps { get; set; }

        public string codigo_obra { get; set; }

        public string art { get; set; }

        public Prestador prestador { get; set; }

        public Tomador tomador { get; set; }

        public Servico servico { get; set; }
    }
}