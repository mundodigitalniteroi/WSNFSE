﻿namespace NFSE.Domain.Entities.NFe
{
    public class PrestadorServico
    {
        public string id_nfse_prestador { get; set; }

        public string prestador_cnpj { get; set; }

        public string prestador_nome { get; set; }

        public string prestador_inscricao_municipal { get; set; }

        public string prestador_codigo_municipio_ibge { get; set; }

        public string prestador_chave { get; set; }

        public string server { get; set; }

        public string _base { get; set; }

        public string connection { get; set; }
    }
}