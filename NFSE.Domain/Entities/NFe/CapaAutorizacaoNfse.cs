namespace NFSE.Domain.Entities.NFe
{
    public class CapaAutorizacaoNfse
    {
        public Autorizacao Autorizacao { get; set; }

        public int GrvId { get; set; }

        public string IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public bool Homologacao { get; set; }
    }
}