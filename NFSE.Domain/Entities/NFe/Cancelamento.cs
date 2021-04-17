namespace NFSE.Domain.Entities.NFe
{
    public class Cancelamento
    {
        public int GrvId { get; set; }

        public string IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public string Justificativa { get; set; }

        public bool Homologacao { get; set; }

        public string Cnpj { get; set; }
    }
}