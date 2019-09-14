namespace NFSE.Domain.Entities
{
    public class Consulta
    {
        public int GrvId { get; set; }

        public int IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public string CnpjPrestador { get; set; }

        public bool Homologacao { get; set; }
    }
}