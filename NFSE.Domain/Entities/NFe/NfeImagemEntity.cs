namespace NFSE.Domain.Entities.NFe
{
    public class NfeImagemEntity
    {
        public int NfeImagemId { get; set; }

        public int NfeId { get; set; }

        public byte[] Imagem { get; set; }
    }
}