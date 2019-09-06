namespace NFSE.Domain.Entities
{
    public class Consulta
    {
        public int UsuarioId { get; set; }

        public int CodigoRetorno { get; set; }

        public string CnpjPrestador { get; set; }

        public bool Homologacao { get; set; }
    }
}