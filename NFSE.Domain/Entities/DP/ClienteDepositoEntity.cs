using System;

namespace NFSE.Domain.Entities.DP
{
    public class ClienteDepositoEntity
    {
        public int ClienteDepositoId { get; set; }

        public int ClienteId { get; set; }

        public int DepositoId { get; set; }

        public int EmpresaId { get; set; }

        public char FlagValorIssIgualProdutoBaseCalculoAliquota { get; set; }
    }
}