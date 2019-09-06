namespace NFSE.Domain.Entities
{
    public class Cancelamento
    {
        public int UsuarioId { get; set; }

        public int IdentificadorNota { get; set; }

        public string CnpjPrestador { get; set; }

        public string Justificativa { get; set; }

        public bool Homologacao { get; set; }
    }
}