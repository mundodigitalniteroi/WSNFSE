using System;

namespace NFSE.Domain.Entities
{
    public class Erro
    {
        public int NotaFiscalErroId { get; set; }

        public int UsuarioId { get; set; }

        public int? GrvId { get; set; }

        public int? CodigoRetorno { get; set; }

        public char OrigemErro { get; set; }

        public string MensagemErro { get; set; }

        public DateTime DataHoraCadastro { get; set; }
    }
}