namespace NFSE.Domain.Entities
{
    public class Consultar
    {
        public string referencia { get; set; }

        public string cnpj_prestador { get; set; }

        public bool homologacao { get; set; }

        public int id_usuario { get; set; }
    }
}