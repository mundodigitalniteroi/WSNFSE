using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeRetornoModel
    {
        public int NotaFiscalId { get; set; }

        public int NfeId { get; set; }

        public int UsuarioId { get; set; }

        public string Status { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public string CodigoVerificacao { get; set; }

        public string UrlNotaFiscal { get; set; }

        public string CaminhoXmlNotaFiscal { get; set; }

        public byte[] ImagemNotaFiscal { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}