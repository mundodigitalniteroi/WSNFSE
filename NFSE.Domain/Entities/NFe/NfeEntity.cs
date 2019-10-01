using System;

namespace NFSE.Domain.Entities.NFe
{
    public class NfeEntity
    {
        public int NfeId { get; set; }

        public int GrvId { get; set; }

        public int FaturamentoServicoTipoVeiculoId { get; set; }

        public int IdentificadorNota { get; set; }

        public int? NfeComplementarId { get; set; }

        public int UsuarioCadastroId { get; set; }

        public string Cnpj { get; set; }

        public string Numero { get; set; }

        public string CodigoVerificacao { get; set; }

        public int? CodigoRetorno { get; set; }

        public string Url { get; set; }

        // STATUS:
        //   C: Cadastro;
        //   A: Aguardando Processamento (envio da solicitação com sucesso, para a Prefeitura);
        //   P: Processado (download da Nfe e atualização da Nfe no Sistema concluídos com sucesso);
        //   R: Reprocessar (marcação manual para o envio de uma nova solicitação de Nfe para o mesmo GRV, esta opção gera um novo registro de Nfe);
        //   S: Aguardando Reprocessamento;
        //   T: Reprocessado (conclusão do reprocessamento);
        //   N: CaNcelado.
        //   E: Erro (quando a Prefeitura indicou algum problema);
        //   I: Inválido (quando ocorreu um erro Mob-Link);
        public char Status { get; set; }

        public string StatusNfe { get; set; }

        public DateTime? DataEmissao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        // Informações para consulta:
        public string Cliente { get; set; }

        public string Deposito { get; set; }
    }
}