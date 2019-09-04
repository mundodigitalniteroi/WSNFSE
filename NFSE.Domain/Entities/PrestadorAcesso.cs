namespace NFSE.Domain.Entities
{
    public class PrestadorAcesso
    {
        public string id_nfse_prestador { get; set; }

        public string prestador_cnpj { get; set; }

        public string prestador_nome { get; set; }

        public string prestador_inscricao_municipal { get; set; }

        public string prestador_codigo_municipio_ibge { get; set; }

        public string prestador_chave { get; set; }

        public string item_lista_servico { get; set; }

        public string codigo_tributario_municipio { get; set; }

        public string codigo_cnae { get; set; }

        public string server { get; set; }

        public string _base { get; set; }

        public string connection { get; set; }
    }
}