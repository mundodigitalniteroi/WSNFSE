using System;

namespace NFSE.Domain.Entities.DP
{
    public class ClienteEntity
    {
        public int ClienteId { get; set; }

        public short AgenciaBancariaId { get; set; }

        public int CepId { get; set; }

        public byte? TipoLogradouroId { get; set; }

        public int? BairroId { get; set; }

        public byte? TipoMeioCobrancaId { get; set; }

        public int? EmpresaId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Nome { get; set; }

        public string Cnpj { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public int? GpsLatitude { get; set; }

        public int? GpsLongitude { get; set; }

        public int? MetragemTotal { get; set; }

        public int? MetragemGuarda { get; set; }

        public string HoraDiaria { get; set; }

        public short MaximoDiariasParaCobranca { get; set; }

        public short MaximoDiasVencimento { get; set; }

        public string CodigoSap { get; set; }

        public string LabelClienteCodigoIdentificacao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public char FlagUsarHoraDiaria { get; set; }

        public char FlagEmissaoNotaFiscalSap { get; set; }

        public char FlagCadastrarQuilometragem { get; set; }

        public char FlagCobrarDiariasDiasCorridos { get; set; }

        public char FlagClienteRealizaFaturamentoArrecadacao { get; set; }

        public char FlagEnderecoCadastroManual { get; set; }

        public char FlagPermiteAlteracaoTipoVeiculo { get; set; }

        public char FlagLancarIpvaMultas { get; set; }

        public char FlagPossuiClienteCodigoIdentificacao { get; set; }

        public char FlagAtivo { get; set; }

        public int? OrgaoExecutivoTransitoId { get; set; }

        public string CodigoOrgao { get; set; }
    }
}