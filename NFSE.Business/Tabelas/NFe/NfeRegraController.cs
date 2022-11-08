﻿using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfeRegraController
    {
        public List<NfeRegraEntity> Listar(NfeRegraEntity model)
        {
            var SQL = new StringBuilder();

            SQL.AppendLine("SELECT vw_dep_nfe_regras.NfeRegraId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.NfeRegraTipoId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.ClienteId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.DepositoId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.ClienteDepositoId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.UsuarioCadastroId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.UsuarioAlteracaoId");

            SQL.AppendLine("      ,vw_dep_nfe_regras.Valor");

            SQL.AppendLine("      ,vw_dep_nfe_regras.DataCadastro");

            SQL.AppendLine("      ,vw_dep_nfe_regras.DataAlteracao");

            SQL.AppendLine("      ,vw_dep_nfe_regras.Ativo");

            SQL.AppendLine("      ,vw_dep_nfe_regras.RegraCodigo");

            SQL.AppendLine("      ,vw_dep_nfe_regras.RegraDescricao");

            SQL.AppendLine("      ,vw_dep_nfe_regras.RegraPossuiValor");

            SQL.AppendLine("      ,vw_dep_nfe_regras.RegraAtivo");

            SQL.AppendLine("      ,vw_dep_nfe_regras.Cliente");

            SQL.AppendLine("      ,vw_dep_nfe_regras.Deposito");

            SQL.AppendLine("      ,vw_dep_nfe_regras.UsuarioCadastro");

            SQL.AppendLine("      ,vw_dep_nfe_regras.UsuarioAlteracao");

            SQL.AppendLine("  FROM dbo.vw_dep_nfe_regras");

            if (model != null)
            {
                SQL.AppendLine(" WHERE 1 = 1");

                if (model.NfeRegraId > 0)
                {
                    SQL.Append("   AND vw_dep_nfe_regras.NfeRegraId = ").Append(model.NfeRegraId).AppendLine();
                }

                if (model.NfeRegraTipoId > 0)
                {
                    SQL.Append("   AND vw_dep_nfe_regras.NfeRegraTipoId = ").Append(model.NfeRegraTipoId).AppendLine();
                }

                if (model.ClienteId > 0)
                {
                    SQL.Append("   AND vw_dep_nfe_regras.ClienteId = ").Append(model.ClienteId).AppendLine();
                }

                if (model.DepositoId > 0)
                {
                    SQL.Append("   AND vw_dep_nfe_regras.DepositoId = ").Append(model.DepositoId).AppendLine();
                }

                if (model.ClienteDepositoId > 0)
                {
                    SQL.Append("   AND vw_dep_nfe_regras.ClienteDepositoId = ").Append(model.ClienteDepositoId).AppendLine();
                }
            }

            using (var dataTable = DataBase.Select(SQL))
            {
                return DataTableUtil.DataTableToList<NfeRegraEntity>(dataTable);
            }
        }


        public NfeRegraEntity Selecionar(NfeRegraEntity model)
        {
            var list = Listar(model);

            return list?.FirstOrDefault();
        }
    }
}
