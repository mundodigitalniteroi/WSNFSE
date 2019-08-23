namespace Negocio.Modelo
{
  public class Servico
  {
    public string aliquota { get; set; }

    public string discriminacao { get; set; }

    public string iss_retido { get; set; }

    public string item_lista_servico { get; set; }

    public string codigo_tributario_municipio { get; set; }

    public string valor_servicos { get; set; }

    public string valor_deducoes { get; set; }

    public string valor_pis { get; set; }

    public string valor_cofins { get; set; }

    public string valor_inss { get; set; }

    public string valor_ir { get; set; }

    public string valor_csll { get; set; }

    public string valor_iss { get; set; }

    public string valor_iss_retido { get; set; }

    public string outras_retencoes { get; set; }

    public string base_calculo { get; set; }

    public string desconto_incondicionado { get; set; }

    public string desconto_condicionado { get; set; }

    public string codigo_cnae { get; set; }

    public string codigo_municipio { get; set; }

    public string percentual_total_tributos { get; set; }

    public string fonte_total_tributos { get; set; }
  }
}
