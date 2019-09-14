using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeMensagemEntity
    {
        public int NfeMensagemId { get; set; }

        public int NfeId { get; set; }

        public string Mensagem { get; set; }

        public char Tipo { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}