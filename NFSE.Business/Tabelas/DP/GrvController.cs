using NFSE.Domain.Entities.DP;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.DP
{
    public class GrvController
    {
        public List<GrvEntity> Listar(GrvEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_grv.id_grv AS GrvId");

            SQL.AppendLine("      ,tb_dep_grv.id_tarifa_tipo_veiculo AS TarifaTipoVeiculoId");

            SQL.AppendLine("      ,tb_dep_grv.id_cliente AS ClienteId");

            SQL.AppendLine("      ,tb_dep_grv.id_deposito AS DepositoId");

            SQL.AppendLine("      ,tb_dep_grv.id_tipo_veiculo AS TipoVeiculoId");

            SQL.AppendLine("      ,tb_dep_grv.id_reboquista AS ReboquistaId");

            SQL.AppendLine("      ,tb_dep_grv.id_reboque AS ReboqueId");

            SQL.AppendLine("      ,tb_dep_grv.id_autoridade_responsavel AS AutoridadeResponsavelId");

            SQL.AppendLine("      ,tb_dep_grv.id_cor AS CorId");

            SQL.AppendLine("      ,tb_dep_grv.id_cor_ostentada AS CorOstentadaId");

            SQL.AppendLine("      ,tb_dep_grv.id_detran_marca_modelo AS DetranMarcaModeloId");

            SQL.AppendLine("      ,tb_dep_grv.id_cep AS CepId");

            SQL.AppendLine("      ,tb_dep_grv.id_motivo_apreensao AS MotivoApreensaoId");

            SQL.AppendLine("      ,tb_dep_grv.id_status_operacao AS StatusOperacaoId");

            SQL.AppendLine("      ,tb_dep_grv.id_liberacao AS LiberacaoId");

            SQL.AppendLine("      ,tb_dep_grv.id_usuario_cadastro AS UsuarioCadastroId");

            SQL.AppendLine("      ,tb_dep_grv.id_usuario_alteracao AS UsuarioAlteracaoId");

            SQL.AppendLine("      ,tb_dep_grv.id_usuario_edicao AS UsuarioEdicaoId");

            SQL.AppendLine("      ,tb_dep_grv.id_usuario_cadastro_ggv AS UsuarioCadastroGgvId");

            SQL.AppendLine("      ,tb_dep_grv.numero_formulario_grv AS NumeroFormularioGrv");

            SQL.AppendLine("      ,tb_dep_grv.faturamento_produto_codigo AS FaturamentoProdutoCodigo");

            SQL.AppendLine("      ,tb_dep_grv.matricula_autoridade_responsavel AS MatriculaAutoridadeResponsavel");

            SQL.AppendLine("      ,tb_dep_grv.nome_autoridade_responsavel AS NomeAutoridadeResponsavel");

            SQL.AppendLine("      ,tb_dep_grv.placa AS Placa");

            SQL.AppendLine("      ,tb_dep_grv.placa_ostentada AS PlacaOstentada");

            SQL.AppendLine("      ,tb_dep_grv.chassi AS Chassi");

            SQL.AppendLine("      ,tb_dep_grv.renavam AS Renavam");

            SQL.AppendLine("      ,tb_dep_grv.rfid AS Rfid");

            SQL.AppendLine("      ,tb_dep_grv.logradouro AS Logradouro");

            SQL.AppendLine("      ,tb_dep_grv.numero AS Numero");

            SQL.AppendLine("      ,tb_dep_grv.complemento AS Complemento");

            SQL.AppendLine("      ,tb_dep_grv.bairro AS Bairro");

            SQL.AppendLine("      ,tb_dep_grv.municipio AS Municipio");

            SQL.AppendLine("      ,tb_dep_grv.uf AS Uf");

            SQL.AppendLine("      ,tb_dep_grv.referencia AS Referencia");

            SQL.AppendLine("      ,tb_dep_grv.ponto_referencia AS PontoReferencia");

            SQL.AppendLine("      ,tb_dep_grv.numero_chave AS NumeroChave");

            SQL.AppendLine("      ,tb_dep_grv.estacionamento_setor AS EstacionamentoSetor");

            SQL.AppendLine("      ,tb_dep_grv.estacionamento_numero_vaga AS EstacionamentoNumeroVaga");

            SQL.AppendLine("      ,tb_dep_grv.divergencia1 AS Divergencia1");

            SQL.AppendLine("      ,tb_dep_grv.divergencia2 AS Divergencia2");

            SQL.AppendLine("      ,tb_dep_grv.divergencia3 AS Divergencia3");

            SQL.AppendLine("      ,tb_dep_grv.divergencia4 AS Divergencia4");

            SQL.AppendLine("      ,tb_dep_grv.divergencia5 AS Divergencia5");

            SQL.AppendLine("      ,tb_dep_grv.latitude AS Latitude");

            SQL.AppendLine("      ,tb_dep_grv.longitude AS Longitude");

            SQL.AppendLine("      ,tb_dep_grv.numero_oficio AS NumeroOficio");

            SQL.AppendLine("      ,tb_dep_grv.matricula_comandante AS MatriculaComandante");

            SQL.AppendLine("      ,tb_dep_grv.data_oficio AS DataOficio");

            SQL.AppendLine("      ,tb_dep_grv.data_hora_remocao AS DataHoraRemocao");

            SQL.AppendLine("      ,tb_dep_grv.data_hora_guarda AS DataHoraGuarda");

            SQL.AppendLine("      ,tb_dep_grv.data_cadastro AS DataCadastro");

            SQL.AppendLine("      ,tb_dep_grv.data_alteracao AS DataAlteracao");

            SQL.AppendLine("      ,tb_dep_grv.flag_comboio AS FlagComboio");

            SQL.AppendLine("      ,tb_dep_grv.flag_veiculo_nao_identificado AS FlagVeiculoNaoIdentificado");

            SQL.AppendLine("      ,tb_dep_grv.flag_veiculo_sem_registro AS FlagVeiculoSemRegistro");

            SQL.AppendLine("      ,tb_dep_grv.flag_veiculo_roubado_furtado AS FlagVeiculoRoubadoFurtado");

            SQL.AppendLine("      ,tb_dep_grv.flag_chave_deposito AS FlagChaveDeposito");

            SQL.AppendLine("      ,tb_dep_grv.flag_estado_lacre AS FlagEstadoLacre");

            SQL.AppendLine("      ,tb_dep_grv.flag_veiculo_mesmas_condicoes AS FlagVeiculoMesmasCondicoes");

            SQL.AppendLine("      ,tb_dep_grv.flag_ggv AS FlagGgv");

            SQL.AppendLine("      ,tb_dep_grv.flag_vistoria AS FlagVistoria");

            SQL.AppendLine("      ,tb_dep_grv.termo_detran AS TermoDetran");

            SQL.AppendLine("      ,tb_dep_grv.flag_veiculo_nao_ostenta_placa AS FlagVeiculoNaoOstentaPlaca");

            SQL.AppendLine("      ,tb_dep_grv.flag_transbordo AS FlagTransbordo");

            SQL.AppendLine("      ,tb_dep_grv.data_transbordo AS DataTransbordo");

            SQL.AppendLine("  FROM dbo.tb_dep_grv");

            if (model.GrvId > 0)
            {
                SQL.AppendLine(" WHERE tb_dep_grv.id_grv = " + model.GrvId);
            }
            else
            {
                SQL.AppendLine(" WHERE tb_dep_grv.numero_formulario_grv = '" + model.NumeroFormularioGrv + "'");
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable != null ? DataTableUtil.DataTableToList<GrvEntity>(dataTable) : null;
            }
        }

        public GrvEntity Selecionar(int grvId)
        {
            var list = Listar(new GrvEntity { GrvId = grvId } );

            return list?.FirstOrDefault();
        }

        public GrvEntity Selecionar(string numeroFormularioGrv)
        {
            var list = Listar(new GrvEntity { NumeroFormularioGrv = numeroFormularioGrv });

            return list?.FirstOrDefault();
        }
    }
}