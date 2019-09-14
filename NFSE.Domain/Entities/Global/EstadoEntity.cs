namespace NFSE.Domain.Entities.Global
{
    public class EstadoEntity
    {
        public string Uf { get; set; }

        public string PaisNumcode { get; set; }

        public string Regiao { get; set; }

        public string Nome { get; set; }

        public string NomePtbr { get; set; }

        public string Capital { get; set; }

        public byte UtcId { get; set; }

        public byte? UtcVeraoId { get; set; }
    }
}