namespace NFSE.Domain.Entities
{
    public class Endereco
    {
        public string logradouro { get; set; }

        public string tipo_logradouro { get; set; }

        public string numero { get; set; }

        public string complemento { get; set; }

        public string bairro { get; set; }

        public string codigo_municipio { get; set; }

        public string uf { get; set; }

        public string cep { get; set; }
    }
}