using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfseMassivoController
    {
        public List<NfeEntity> ListarPorIdentificadorNota(int identificadorNota)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe.Id as NfeId");

            SQL.AppendLine("      ,tb_dep_nfe.IdentificadorNota");

            SQL.AppendLine("      ,tb_dep_nfe.Cnpj");

            SQL.AppendLine("      ,tb_dep_nfe.Numero");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoVerificacao");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoRetorno");

            SQL.AppendLine("      ,tb_dep_nfe.Url");

            SQL.AppendLine("      ,tb_dep_nfe.Status");

            SQL.AppendLine("      ,tb_dep_nfe.DataEmissao");

            SQL.AppendLine("      ,tb_dep_nfe.DataCadastro");

            SQL.AppendLine("      ,tb_dep_nfe.DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_nfse");

            SQL.AppendLine(" WHERE IdentificadorNota = @IdentificadorNota");

            SqlParameter[] sqlParameters =
            {
                new SqlParameter("@IdentificadorNota",SqlDbType.Int) { Value = identificadorNota }
            };

            using (var dataTable = DataBase.Select(SQL, sqlParameters))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public List<NfeEntity> Listar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT tb_dep_nfe.Id as NfeId");

            SQL.AppendLine("      ,tb_dep_nfe.IdentificadorNota");

            SQL.AppendLine("      ,tb_dep_nfe.Cnpj");

            SQL.AppendLine("      ,tb_dep_nfe.Numero");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoVerificacao");

            SQL.AppendLine("      ,tb_dep_nfe.CodigoRetorno");

            SQL.AppendLine("      ,tb_dep_nfe.Url");

            SQL.AppendLine("      ,tb_dep_nfe.Status");

            SQL.AppendLine("      ,tb_dep_nfe.DataEmissao");

            SQL.AppendLine("      ,tb_dep_nfe.DataCadastro");

            SQL.AppendLine("      ,tb_dep_nfe.DataAlteracao");

            SQL.AppendLine("  FROM dbo.tb_nfse");

            SQL.AppendLine(" WHERE 1 = 1");

            if (model.IdentificadorNota > 0)
            {
                SQL.AppendLine("   AND IdentificadorNota = " + model.IdentificadorNota);
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfeEntity>(dataTable);
            }
        }

        public NfeEntity Selecionar(NfeEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }

        public int Cadastrar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("INSERT INTO dbo.tb_nfse");

            SQL.AppendLine("      (Cnpj");
            SQL.AppendLine("      ,IdentificadorNota");
            SQL.AppendLine("      ,StatusNfe)");

            SQL.AppendLine("VALUES");

            SQL.AppendLine("      ('" + model.Cnpj + "'");
            SQL.AppendLine("      ," + model.IdentificadorNota);
            SQL.AppendLine("      ,'" + model.StatusNfe + "')");

            return DataBase.ExecuteScopeIdentity(SQL);
        }

        public int Atualizar(NfeEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_nfse");

            SQL.AppendLine("   SET Status = '" + model.Status + "'");

            if (model.CodigoRetorno != null && model.CodigoRetorno > 0)
            {
                SQL.AppendLine("       ,CodigoRetorno = " + model.CodigoRetorno);
            }

            SQL.AppendLine("      ,DataAlteracao = GETDATE()");

            SQL.AppendLine(" WHERE Id = " + model.NfeId);

            return DataBase.Execute(SQL);
        }

        public int AtualizarRetornoNotaFiscal(NfeEntity nfe, RetornoNotaFiscalEntity retornoNotaFiscal)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_nfse");

            SQL.AppendLine("   SET Referencia = '" + retornoNotaFiscal.@ref.Trim() + "'");

            if (retornoNotaFiscal.numero_rps != null)
            {
                SQL.AppendLine("      ,Numero = '" + retornoNotaFiscal.numero_rps.Trim() + "'");

                SQL.AppendLine("      ,NumeroRps = '" + retornoNotaFiscal.numero_rps.Trim() + "'");
            }

            if (retornoNotaFiscal.serie_rps != null)
            {
                SQL.AppendLine("      ,SerieRps = '" + retornoNotaFiscal.serie_rps.Trim() + "'");
            }

            SQL.AppendLine("      ,Status = '" + retornoNotaFiscal.status.Trim() + "'");

            SQL.AppendLine("      ,NumeroNotaFiscal = '" + retornoNotaFiscal.numero.Trim() + "'");

            SQL.AppendLine("      ,CodigoVerificacao = '" + retornoNotaFiscal.codigo_verificacao.Trim() + "'");

            SQL.AppendLine("      ,DataEmissao = '" + retornoNotaFiscal.data_emissao.ToString("yyyyMMdd HH:mm:ss") + "'");

            SQL.AppendLine("      ,Url = '" + retornoNotaFiscal.url.Trim() + "'");

            SQL.AppendLine("      ,CaminhoXmlNotaFiscal = '" + retornoNotaFiscal.caminho_xml_nota_fiscal.Trim() + "'");

            SQL.AppendLine(" WHERE Id = " + nfe.NfeId);

            return DataBase.Execute(SQL);
        }

        public NfeEntity ConsultarNotaFiscal(int identificadorNota)
        {
            List<NfeEntity> nfe;

            try
            {
                if ((nfe = ListarPorIdentificadorNota(identificadorNota)) == null)
                {
                    throw new Exception("Nota Fiscal não encontrada (" + identificadorNota + ")");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao consultar a Nota Fiscal (" + identificadorNota + "): " + ex.Message);
            }

            return nfe.FirstOrDefault();
        }

        public List<NfseDados> ListarDados()
        {
            var SQL = new StringBuilder();

            SQL.AppendLine(@"SELECT top 10
                                Id, 
                                Lote,
                                Leilao,
                                Status,
                                Placa,
                                Chassi,
                                Renavam,
                                MarcaModelo,
                                Cor,
                                Combustivel,
                                AnoFab,
                                AnoMod,
                                NumeroMotor,
                                DataApreensao,
                                UfVeiculo,
                                dbo.StripNonNumerics(CpfCnpj) as CpfCnpj,
                                Nome,
                                dbo.StripNonNumerics(Telefone) as Telefone,
                                dbo.StripNonNumerics(Celular) as Celular,
                                Email,
                                Logradouro,
                                Numero,
                                Complemento,
                                Bairro,
                                Cidade,
                                Uf,
                                Cep,
                                CodigoMunicipioIbge,
                                NotaCriada,
                                ValorServico
                                FROM dbo.tb_nfse_dados WHERE NotaCriada = 0");

            using (var dataTable = DataBase.Select(SQL))
            {
                return dataTable == null ? null : DataTableUtil.DataTableToList<NfseDados>(dataTable);
            }
        }

        public int AtualizarDados(NfseDados model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("UPDATE dbo.tb_nfse_dados");

            SQL.AppendLine(" SET NotaCriada = " + (model.NotaCriada ? 1 : 0));
            SQL.AppendLine(" ,IdentificadorNota = " + model.IdentificadorNota);

            SQL.AppendLine(" WHERE Id = " + model.Id);

            return DataBase.Execute(SQL);
        }
    }
}