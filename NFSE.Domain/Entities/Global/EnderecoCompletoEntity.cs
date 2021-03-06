﻿namespace NFSE.Domain.Entities.Global
{
    public class EnderecoCompletoEntity
    {
        public int CepId { get; set; }

        public int MunicipioId { get; set; }

        public int? BairroId { get; set; }

        public byte? TipoLogradouroId { get; set; }

        public string Cep { get; set; }

        public string TipoLogradouro { get; set; }

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string BairroPtbr { get; set; }

        public string Municipio { get; set; }

        public string MunicipioPtbr { get; set; }

        public string CodigoMunicipio { get; set; }

        public string CodigoMunicipioIbge { get; set; }

        public string Estado { get; set; }

        public string EstadoPtbr { get; set; }

        public string Uf { get; set; }

        public string Regiao { get; set; }

        public string RegiaoNome { get; set; }

        public char FlagSanitizado { get; set; }
    }
}