// using Newtonsoft.Json;

namespace NFSE.Domain.Entities.NFe
{
    public class Autorizacao
    {
        // [JsonProperty("data_emissao")]
        public string data_emissao { get; set; }

        public byte natureza_operacao { get; set; } = 1;

        public string regime_especial_tributacao { get; set; }

        public bool optante_simples_nacional { get; set; }

        public string incentivador_cultural { get; set; }

        public string tributacao_rps { get; set; }

        public string codigo_obra { get; set; }

        public string art { get; set; }

        public byte consumidor_final { get; set; } = 0;
        public decimal percentual_total_tributos_simples_nacional { get; set; }
        public byte indicador_destinatario { get; set; } = 0;

        public Prestador prestador { get; set; }

        public Tomador tomador { get; set; }

        public Servico servico { get; set; }
    }
}