using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Diagnostics;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBase.SystemEnvironment = SystemEnvironment.Production;

            bool IsTestEnvironment = DataBase.SystemEnvironment.Equals('D') ? true : false;

            DataBase.ConnectDataBase();

            var grv = new GrvController().Selecionar("906010300");

            if (grv != null)
            {
                Debug.WriteLine($"GRV ID: {grv.GrvId}");

                Console.WriteLine($"GRV ID: {grv.GrvId}");
            }

            #region Teste de Solicitação Simplificado
            //try
            //{
            //    var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
            //    (
            //        grvId: grv.GrvId, 

            //        usuarioId: 1, 

            //        isDev: IsTestEnvironment
            //    );

            //    for (int i = 0; i < nfe.Count; i++)
            //    {
            //        Console.WriteLine("MENSAGEM: " + nfe[i]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de Solicitação Simplificado


            #region Teste de Solicitação de uma nova NF
            try
            {
                var novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: grv.GrvId,

                    identificadorNota: 733774,

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
            //try
            //{
            //    var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
            //    {
            //        GrvId = grv.GrvId,

            //        IdentificadorNota = 732155,

            //        Homologacao = IsTestEnvironment,

            //        UsuarioId = 1
            //    });

            //    Console.WriteLine("MENSAGEM: " + aux);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de retorno da Nota Fiscal (Download da NF)


            #region Teste de cancelamento da Nota Fiscal
            //try
            //{
            //    var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
            //    {
            //        GrvId = grv.GrvId,

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