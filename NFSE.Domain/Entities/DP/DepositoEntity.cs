using System;

namespace NFSE.Domain.Entities.DP
{
    public class DepositoEntity
    {
        public int DepositoId { get; set; }

        public int EmpresaId { get; set; }

        public int? CepId { get; set; }

        public int? TipoLogradouroId { get; set; }

        public int? BairroId { get; set; }

        public int? SistemaExternoId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Descricao { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string EmailNfe { get; set; }

        public int GrvMinimoFotosExigidas { get; set; }

        public int GrvLimiteMinimoDatahoraGuarda { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string EnderecoMob { get; set; }

        public string TelefoneMob { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public char FlagEnderecoCadastroManual { get; set; }

        public char FlagAtivo { get; set; }

        public char? FlagVirtual { get; set; }
    }
}