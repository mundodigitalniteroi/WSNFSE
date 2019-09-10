using System;

namespace NFSE.Domain.Entities
{
    public class NfeWsErroModel
    {
        public int ErroId { get; set; }

        public int IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public char Acao { get; set; }

        public char OrigemErro { get; set; }

        public string Status { get; set; }

        public string CodigoErro { get; set; }

        public string MensagemErro { get; set; }

        public string CorrecaoErro { get; set; }

        public DateTime DataHoraCadastro { get; set; }
    }
}