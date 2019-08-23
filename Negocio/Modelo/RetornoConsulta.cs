using System;

namespace Negocio.Modelo
{
  public class RetornoConsulta
  {
    public string cnpj_prestador { get; set; }

    public string @ref { get; set; }

    public string status { get; set; }

    public string numero { get; set; }

    public string codigo_verificacao { get; set; }

    public DateTime data_emissao { get; set; }

    public string url { get; set; }

    public string caminho_xml_nota_fiscal { get; set; }

    public byte[] retorno_nota { get; set; }
  }
}
