using System;

namespace NFSE.Domain.Entities.Global
{
    public class EmpresaEntity
    {
        public int EmpresaId { get; set; }

        public int? EmpresaMatrizId { get; set; }

        public byte EmpresaClassificacaoId { get; set; }

        public int? CepId { get; set; }

        public byte? TipoLogradouroId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public string Cnpj { get; set; }

        public string Nome { get; set; }

        public string NomeFantasia { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Municipio { get; set; }

        public string Uf { get; set; }

        public short? CodigoAlterdata { get; set; }

        public string CodigoSap { get; set; }

        public int? CnaeId { get; set; }

        public int? CnaeListaServicoId { get; set; }

        public string InscricaoEstadual { get; set; }

        public string InscricaoMunicipal { get; set; }

        public short? CodigoTributarioMunicipio { get; set; }

        public string Token { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public char FlagIssRetido { get; set; }

        public char FlagAtivo { get; set; }

        public char OptanteSimplesNacional { get; set; }
    }
}