namespace NFSE.Domain.Entities
{
    public class CapaAutorizacaoNfse
    {
        public Autorizacao Autorizacao { get; set; }

        public int GrvId { get; set; }

        public int IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public bool Homologacao { get; set; }
    }
}