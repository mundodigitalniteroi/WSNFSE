namespace NFSE.Domain.Entities.NFe
{
    public class Consulta
    {
        public int GrvId { get; set; }

        public int NfeId { get; set; }

        public int IdentificadorNota { get; set; }

        public int UsuarioId { get; set; }

        public bool Homologacao { get; set; }

        public bool BaixarImagemOriginal { get; set; } = false;

        public string Cnpj { get; set; }
    }
}