using System;
using System.Collections.Generic;

namespace NFSE.Domain.Entities.NFe
{
    public class RetornoNotaFiscalEntity
    {
        public string cnpj_prestador { get; set; }

        public string @ref { get; set; }

        public string numero_rps { get; set; }

        public string serie_rps { get; set; }

        public string status { get; set; }

        public string codigo_verificacao { get; set; }
         
        public DateTime data_emissao { get; set; }
         
        public string url { get; set; }
         
        public string caminho_xml_nota_fiscal { get; set; }
         
        public byte[] ImagemNotaFiscal { get; set; }
         
        public byte[] retorno_nota { get; set; }

        public List<Erros> erros { get; set; }
    }
}