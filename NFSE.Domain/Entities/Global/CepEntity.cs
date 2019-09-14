namespace NFSE.Domain.Entities.Global
{
    public class CepEntity
    {
        public int CepId { get; set; }

        public int MunicipioId { get; set; }

        public int? BairroId { get; set; }

        public byte? TipoLogradouroId { get; set; }

        public string Cep { get; set; }

        public string Logradouro { get; set; }

        public char FlagSanitizado { get; set; }
    }
}