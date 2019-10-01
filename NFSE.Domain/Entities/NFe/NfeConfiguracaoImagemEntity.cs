using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeConfiguracaoImagemEntity
    {
        public int ConfiguracaoImagemId { get; set; }

        public int ClienteDepositoId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public int? UsuarioAlteracaoId { get; set; }

        public int ValueX { get; set; }

        public int ValueY { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        // Para consultas:
        public int ClienteId { get; set; }

        public int DepositoId { get; set; }

    }
}