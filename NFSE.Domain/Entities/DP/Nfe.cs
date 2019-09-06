using System;

namespace NFSE.Domain.Entities.DP
{
    public class Nfe
    {
        public int NfeID { get; set; }

        public int GrvID { get; set; }

        public int UsuarioCadastroID { get; set; }

        public string Cnpj { get; set; }

        public int? CodigoRetorno { get; set; }

        public string Numero { get; set; }

        public string CodigoVerificacao { get; set; }

        public DateTime? DataEmissao { get; set; }

        public string StatusNfe { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public int? IdentificadorNota { get; set; }
    }
}