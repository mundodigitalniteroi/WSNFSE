using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class ClienteController
    {
        public List<ClienteEntity> Listar(int clienteId)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_clientes.id_cliente AS ClienteId");

            SQL.AppendLine("      ,tb_dep_clientes.id_agencia_bancaria AS AgenciaBancariaId");

            SQL.AppendLine("      ,tb_dep_clientes.id_cep AS CepId");

            SQL.AppendLine("      ,tb_dep_clientes.id_tipo_logradouro AS TipoLogradouroId");

            SQL.AppendLine("      ,tb_dep_clientes.id_bairro AS BairroId");

            SQL.AppendLine("      ,tb_dep_clientes.id_tipo_meio_cobranca AS TipoMeioCobrancaId");

            SQL.AppendLine("      ,tb_dep_clientes.id_empresa AS EmpresaId");

            SQL.AppendLine("      ,tb_dep_clientes.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_clientes.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_clientes.nome AS Nome");

            SQL.AppendLine("      ,tb_dep_clientes.cnpj AS Cnpj");

            SQL.AppendLine("      ,tb_dep_clientes.logradouro AS Logradouro");

            SQL.AppendLine("      ,tb_dep_clientes.numero AS Numero");

            SQL.AppendLine("      ,tb_dep_clientes.complemento AS Complemento");

            SQL.AppendLine("      ,tb_dep_clientes.gps_latitude AS GpsLatitude");

            SQL.AppendLine("      ,tb_dep_clientes.gps_longitude AS GpsLongitude");

            SQL.AppendLine("      ,tb_dep_clientes.metragem_total AS MetragemTotal");

            SQL.AppendLine("      ,tb_dep_clientes.metragem_guarda AS MetragemGuarda");

            SQL.AppendLine("      ,tb_dep_clientes.hora_diaria AS HoraDiaria");

            SQL.AppendLine("      ,tb_dep_clientes.maximo_diarias_para_cobranca AS MaximoDiariasParaCobranca");

            SQL.AppendLine("      ,tb_dep_clientes.maximo_dias_vencimento AS MaximoDiasVencimento");

            SQL.AppendLine("      ,tb_dep_clientes.codigo_sap AS CodigoSap");

            SQL.AppendLine("      ,tb_dep_clientes.label_cliente_codigo_identificacao AS LabelClienteCodigoIdentificacao");

            SQL.AppendLine("      ,tb_dep_clientes.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_clientes.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_clientes.flag_usar_hora_diaria AS FlagUsarHoraDiaria");

            SQL.AppendLine("      ,tb_dep_clientes.flag_emissao_nota_fiscal_sap AS FlagEmissaoNotaFiscalSap");

            SQL.AppendLine("      ,tb_dep_clientes.flag_cadastrar_quilometragem AS FlagCadastrarQuilometragem");

            SQL.AppendLine("      ,tb_dep_clientes.flag_cobrar_diarias_dias_corridos AS FlagCobrarDiariasDiasCorridos");

            SQL.AppendLine("      ,tb_dep_clientes.flag_cliente_realiza_faturamento_arrecadacao AS FlagClienteRealizaFaturamentoArrecadacao");

            SQL.AppendLine("      ,tb_dep_clientes.flag_endereco_cadastro_manual AS FlagEnderecoCadastroManual");

            SQL.AppendLine("      ,tb_dep_clientes.flag_permite_alteracao_tipo_veiculo AS FlagPermiteAlteracaoTipoVeiculo");

            SQL.AppendLine("      ,tb_dep_clientes.flag_lancar_ipva_multas AS FlagLancarIpvaMultas");

            SQL.AppendLine("      ,tb_dep_clientes.flag_possui_cliente_codigo_identificacao AS FlagPossuiClienteCodigoIdentificacao");

            SQL.AppendLine("      ,tb_dep_clientes.flag_ativo AS FlagAtivo");

            SQL.AppendLine("      ,tb_dep_clientes.id_orgao_executivo_transito AS OrgaoExecutivoTransitoId");

            SQL.AppendLine("      ,tb_dep_clientes.codigo_orgao AS CodigoOrgao");

            SQL.AppendLine("  FROM dbo.tb_dep_clientes");

            SQL.Append(" WHERE tb_dep_clientes.id_cliente = ").Append(clienteId).AppendLine();

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<ClienteEntity>(dataTable);
            }
        }

        public ClienteEntity Selecionar(int clienteId)
        {
            var list = Listar(clienteId);

            return list?.FirstOrDefault();
        }
    }
}