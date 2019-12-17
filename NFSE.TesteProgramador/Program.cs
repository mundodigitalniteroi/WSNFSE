using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBase.SystemEnvironment = SystemEnvironment.Development;

            DataBase.ConnectDataBase();

            var grv = new GrvController().Selecionar("14059625");

            Console.WriteLine($"GRV ID: {grv.GrvId}");


            #region Teste de Solicitação Simplificado
            try
            {
                //var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal(grvId: 868480, usuarioId: 1, isDev: TestSystemEnvironment.Development);

                //for (int i = 0; i < nfe.Count; i++)
                //{
                //    Console.WriteLine("MENSAGEM: " + nfe[i]);
                //}
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
                    grvId: 543772,
                    identificadorNota: 700445,
                    usuarioId: 1,
                    isDev: TestSystemEnvironment.Development
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
                //var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = 594289,
                //    IdentificadorNota = 718711,
                //    Homologacao = TestSystemEnvironment.Development,
                //    UsuarioId = 1
                //});

                //Console.WriteLine("MENSAGEM: " + aux);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de retorno da Nota Fiscal (Download da NF)


            #region Teste de cancelamento da Nota Fiscal
            try
            {
                //var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                //{
                //    GrvId = 836989,
                //    IdentificadorNota = 715109,
                //    Justificativa = "TESTES",
                //    Homologacao = TestSystemEnvironment.Development,
                //    UsuarioId = 1
                //});

                //Console.WriteLine("MENSAGEM: " + aux);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de cancelamento da Nota Fiscal

            Console.ReadLine();
        }
    }
}