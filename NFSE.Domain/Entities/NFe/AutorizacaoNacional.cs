using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFSE.Domain.Entities.NFe
{
    public class AutorizacaoNacional
    {
        public string data_emissao { get; set; }
        public string data_competencia { get; set; }
        public string codigo_municipio_emissora { get; set; }
        public string cnpj_prestador { get; set; }
        public int? codigo_opcao_simples_nacional { get; set; } = 1;
        public string regime_especial_tributacao { get; set; }
        public string cnpj_tomador { get; set; }
        public string cpf_tomador { get; set; }
        public string razao_social_tomador { get; set; }
        public string codigo_municipio_tomador { get; set; }
        public string cep_tomador { get; set; }
        public string logradouro_tomador { get; set; }
        public string numero_tomador { get; set; }
        public string complemento_tomador { get; set; }
        public string bairro_tomador { get; set; }
        public string telefone_tomador { get; set; }
        public string email_tomador { get; set; }
        public string codigo_municipio_prestacao { get; set; }
        public string inscricao_municipal_prestador { get; set; }
        public string codigo_tributacao_nacional_iss { get; set; }
        public string codigo_nbs { get; set; }
        public string descricao_servico { get; set; }
        public string valor_servico { get; set; }
        public string valor_iss { get; set; }
        public int? tributacao_iss { get; set; }
        public int? tipo_retencao_iss { get; set; }
        public string percentual_total_tributos_federais { get; set; }
        public string percentual_total_tributos_estaduais { get; set; }
        public string percentual_total_tributos_municipais { get; set; }
        public string situacao_tributaria_pis_cofins { get; set; }
        public string indicador_total_tributacao { get; set; } = "0";

        public string valor_total_tributos_federais { get; set; } = "0";
        public string valor_total_tributos_estaduais { get; set; } = "0";
        public string valor_total_tributos_municipais { get; set; } = "0";

        public byte? consumidor_final { get; set; } = 0;
        public byte? indicador_destinatario { get; set; }
        public int? regime_tributario_simples_nacional { get; set; }
        public decimal? percentual_total_tributos_simples_nacional { get; set; }
        public int? finalidade_emissao { get; set; }

        public string ibs_cbs_classificacao_tributaria { get; set; }
        public string ibs_cbs_situacao_tributaria { get; set; }
        public string codigo_indicador_operacao { get; set; }
        public string codigo_municipio_incidencia { get; set; }

        // Teste - 14/05/2026 - Leonardo
        public string ibs_cbs_base_calculo { get; set; }
        public string ibs_mun_percentual_reducao_aliquota { get; set; }
        public string cbs_percentual_reducao_aliquota { get; set; }
        public string ibs_uf_percentual_reducao_aliquota { get; set; }
        public string cbs_aliquota_efetiva { get; set; }
        public string ibs_valor_total { get; set; }
        public string ibs_uf_valor { get; set; }
        public string ibs_mun_valor { get; set; }
        public string ibs_uf_aliquota { get; set; }
        public string ibs_uf_aliquota_efetiva { get; set; }
        public string cbs_valor { get; set; }
        public string ibs_cbs_codigo_municipio_incidencia { get; set; }
        public string ibs_cbs_descricao_municipio_incidencia { get; set; }
        public string ibs_mun_aliquota_efetiva { get; set; }
        public string ibs_mun_aliquota { get; set; }
        public string cbs_aliquota { get; set; }
        public string ibs_cbs_valor_total { get; set; }

        public string codigo_tributacao_municipal_iss { get; set; }

    }
}