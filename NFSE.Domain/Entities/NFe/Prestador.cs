namespace NFSE.Domain.Entities.NFe
{
    public class Prestador
    {
        public string cnpj { get; set; }

        public string inscricao_estadual { get; set; }

        public string inscricao_municipal { get; set; }

        public string codigo_municipio { get; set; }
        public string nome_municipio { get; set; }
    }
}