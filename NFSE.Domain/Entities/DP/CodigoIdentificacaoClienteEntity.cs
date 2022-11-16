using System;

namespace NFSE.Domain.Entities.DP
{
    public class CodigoIdentificacaoClienteEntity
    {
        public int CodigoIdentificacaoClienteId { get; set; }

        public int GrvId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int UsuarioAlteracaoId { get; set; }

        public string CodigoIdentificacao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime DataAlteracao { get; set; }
    }
}