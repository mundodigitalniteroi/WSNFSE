using System;

namespace NFSE.Domain.Entities.Global
{
    public class ParametroMunicipioEntity
    {
        public int ParametroMunicipioId { get; set; }

        public string CodigoCnae { get; set; }

        public string ItemListaServico { get; set; }

        public string CodigoMunicipioIbge { get; set; }

        public string CodigoTributarioMunicipio { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}