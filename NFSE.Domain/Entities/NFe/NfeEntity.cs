using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeEntity
    {
        public int NfeId { get; set; }

        public int GrvId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public string Cnpj { get; set; }

        public int? CodigoRetorno { get; set; }

        public string Numero { get; set; }

        public string CodigoVerificacao { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string StatusNfe { get; set; }

        public string Url { get; set; }

        public char Status { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public int? IdentificadorNota { get; set; }
    }
}