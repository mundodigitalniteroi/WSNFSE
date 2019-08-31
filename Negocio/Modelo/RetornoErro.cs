namespace Negocio.Modelo
{
    public class RetornoErro
    {
        public int NotaFiscalErroId { get; set; }

        public int AutorizacaoNotaFiscalId { get; set; }

        public int UsuarioId { get; set; }

        public string CodigoErro { get; set; }

        public string MensagemErro { get; set; }
    }
}