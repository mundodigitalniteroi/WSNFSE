using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Diagnostics;
using System.Linq;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBase.SystemEnvironment = SystemEnvironment.Production;

            DataBase.SystemEnvironment = SystemEnvironment.Development;

            bool IsTestEnvironment = DataBase.SystemEnvironment.Equals(SystemEnvironment.Development) ? true : false;

            //DataBase.DisconnectDataBase();

            //if (DataBase.SystemEnvironment == SystemEnvironment.Development)
            //{
            //    DataBase.ConnectDataBase("Data Source=187.84.228.60;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=dp_user_dev;Password=5y3d#%&&!x");
            //}
            //else
            //{
            //    DataBase.ConnectDataBase("Data Source=187.84.228.60;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=dp_user_prd;Password=4y3d#%&&!x");
            //}

            DataBase.ConnectDataBase();

            var grvs = new GrvController().Listar(new GrvEntity { NumeroFormularioGrv = "71104832" });

            if (grvs != null)
            {
                if (grvs.Count > 1)
                {
                    Debug.WriteLine($"FORAM ENCONTRADOS MAIS DE UM GRV");

                    Console.WriteLine($"FORAM ENCONTRADOS MAIS DE UM GRV");

                    Console.ReadLine();

                    Environment.Exit(-1);
                }
            }
            else
            {
                Debug.WriteLine($"NÚMERO DE GRV NÃO ENCONTRADO");

                Console.WriteLine($"NÚMERO DE GRV NÃO ENCONTRADO");

                Console.ReadLine();

                Environment.Exit(-1);
            }

            var grvId = grvs.First().GrvId;

            Debug.WriteLine($"GRV ID: {grvId}");

            Console.WriteLine($"GRV ID: {grvId}");

            // NfeExcluirController.Excluir(749458);

            #region Teste de Solicitação Simplificado
            try
            {
                var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
                (
                    grvId: grvId,

                    usuarioId: 1,

                    isDev: IsTestEnvironment
                );

                for (int i = 0; i < nfe.Count; i++)
                {
                    Console.WriteLine("MENSAGEM: " + nfe[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de Solicitação Simplificado


            #region Teste de Solicitação de uma nova NF
            try
            {
                var novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: grvId,

                    identificadorNota: 753804,

                    usuarioId: 1,

                    isDev: IsTestEnvironment
                );

                Console.WriteLine("MENSAGEM: " + novaNfe[0]);

                novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: grvId,

                    identificadorNota: 753803,

                    usuarioId: 1,

                    isDev: IsTestEnvironment
                );

                Console.WriteLine("MENSAGEM: " + novaNfe[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de Solicitação de uma nova NF


            #region Teste de retorno da Nota Fiscal (Download da NF)
            try
            {
                var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = grvId,

                    IdentificadorNota = 751629,

                    Homologacao = IsTestEnvironment,

                    UsuarioId = 1
                });

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = grvId,

                    IdentificadorNota = 751627,

                    Homologacao = IsTestEnvironment,

                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de retorno da Nota Fiscal (Download da NF)


            #region Teste de cancelamento da Nota Fiscal
            //try
            //{
            //    var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
            //    {
            //        GrvId = grvId,

            //        IdentificadorNota = 0,

            //        Justificativa = "TESTE",

            //        Homologacao = IsTestEnvironment,

            //        UsuarioId = 1
            //    });

            //    Console.WriteLine("MENSAGEM: " + aux);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de cancelamento da Nota Fiscal

            Console.ReadLine();
        }
    }
}