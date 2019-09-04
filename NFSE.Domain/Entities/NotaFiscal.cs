using System;

namespace NFSE.Domain.Entities
{
    public class NotaFiscal
    {
        public int NotaFiscalId { get; set; }

        public int AutorizacaoNotaFiscalId { get; set; }

        public int UsuarioId { get; set; }

        public string FlagAmbiente { get; set; }

        public string StatusNotaFiscal { get; set; }

        public string NumeroNotaFiscal { get; set; }

        public string CodigoVerificacao { get; set; }

        public DateTime DataEmissao { get; set; }

        public string UrlNotaFiscal { get; set; }

        public string CaminhoNotaFiscal { get; set; }

        public byte[] ImagemNotaFiscal { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}