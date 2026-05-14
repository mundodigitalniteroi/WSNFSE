namespace NFSE.Domain.Entities.Global
{
    public class CnaeListaServicoParametroMunicipioEntity
    {
        public int CnaeListaServicoId { get; set; }
        public int CnaeId { get; set; }
        public int ListaServicoId { get; set; }
        public int MunicipioId { get; set; }
        public int ParametroMunicipioId { get; set; }
        public string CnaeCodigo { get; set; }
        public string CnaeCodigoFormatado { get; set; }
        public string CnaeDescricao { get; set; }
        public string ListaServico { get; set; }
        public string ListaServicoDescricao { get; set; }
        public decimal? AliquotaIss { get; set; }
        public string Uf { get; set; }
        public string Municipio { get; set; }
        public string CodigoMunicipioIbge { get; set; }
        public string CodigoTributarioMunicipio { get; set; }
        public string CodigoTributacaoNacionalIss { get; set; }
        public string CodigoNbs { get; set; }
        public bool ItemListaServicoNacional { get; set; } = false;
        public string IbsCbsClassificacaoTributaria { get; set; }
        public string IbsCbsSituacaoTributaria { get; set; }
        public string CodigoIndicadorOperacao { get; set; }
        public byte? ConsumidorFinal { get; set; }
        public byte? IndicadorDestinatario { get; set; }
        public string SituacaoTributariaPisCofins { get; set; }
        public int? TipoRetencaoPisCofins { get; set; }
        public int? TipoRetencaoIss { get; set; }
        public int? CodigoOpcaoSimplesNacional { get; set; }
        public int? TributacaoIss { get; set; }
        public int? RegimeTributarioSimplesNacional { get; set; }
        public decimal? PercentualTotalTributosSimplesNacional { get; set; }
        public int? FinalidadeEmissao { get; set; }
    }
}