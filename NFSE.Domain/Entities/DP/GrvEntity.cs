using System;

namespace NFSE.Domain.Entities.DP
{
    public class GrvEntity
    {
        public int GrvId { get; set; }

        public int? TarifaTipoVeiculoId { get; set; }

        public int ClienteId { get; set; }

        public int DepositoId { get; set; }

        public byte? TipoVeiculoId { get; set; }

        public int? ReboquistaId { get; set; }

        public int? ReboqueId { get; set; }

        public int? AutoridadeResponsavelId { get; set; }

        public int? CorId { get; set; }

        public int? CorOstentadaId { get; set; }

        public int? DetranMarcaModeloId { get; set; }

        public int? CepId { get; set; }

        public byte? MotivoApreensaoId { get; set; }

        public char StatusOperacaoId { get; set; }

        public int? LiberacaoId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public int? UsuarioEdicaoId { get; set; }

        public int? UsuarioCadastroGgvId { get; set; }

        public string NumeroFormularioGrv { get; set; }

        public string FaturamentoProdutoCodigo { get; set; }

        public string MatriculaAutoridadeResponsavel { get; set; }

        public string NomeAutoridadeResponsavel { get; set; }

        public string Placa { get; set; }

        public string PlacaOstentada { get; set; }

        public string Chassi { get; set; }

        public string Renavam { get; set; }

        public string RfId { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Municipio { get; set; }

        public string Uf { get; set; }

        public string Referencia { get; set; }

        public string PontoReferencia { get; set; }

        public string NumeroChave { get; set; }

        public string EstacionamentoSetor { get; set; }

        public string EstacionamentoNumeroVaga { get; set; }

        public string Divergencia1 { get; set; }

        public string Divergencia2 { get; set; }

        public string Divergencia3 { get; set; }

        public string Divergencia4 { get; set; }

        public string Divergencia5 { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string NumeroOficio { get; set; }

        public string MatriculaComandante { get; set; }

        public DateTime? DataOficio { get; set; }

        public DateTime? DataHoraRemocao { get; set; }

        public DateTime? DataHoraGuarda { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public char FlagComboio { get; set; }

        public char FlagVeiculoNaoIdentificado { get; set; }

        public char FlagVeiculoSemRegistro { get; set; }

        public char FlagVeiculoRoubadoFurtado { get; set; }

        public char FlagChaveDeposito { get; set; }

        public char FlagEstadoLacre { get; set; }

        public char FlagVeiculoMesmasCondicoes { get; set; }

        public char FlagGgv { get; set; }

        public char FlagVistoria { get; set; }

        public string TermoDetran { get; set; }

        public char FlagVeiculoNaoOstentaPlaca { get; set; }

        public char FlagTransbordo { get; set; }

        public DateTime? DataTransbordo { get; set; }

        public int? AgenteId { get; set; }

        public string LatitudeAcautelamento { get; set; }

        public string LongitudeAcautelamento { get; set; }

        public int? DistanciaAteAcautelamento { get; set; }
    }
}