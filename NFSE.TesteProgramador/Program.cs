using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.NFe;
using NFSE.Infra.Data;
using NFSE.Domain.Enum;
using System;
using System.Collections.Generic;

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

            //try
            //{
            //    string result = new NfeSolicitarEmissaoNotaFiscalController().SimularEmissaoNotaFiscal(capaAutorizacaoNfse);
                
            //    Console.WriteLine(result);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}

            #region Emissão da Nota Fiscal Eletrônica
            try
            {
                //var nfe = new NfeController().Retornar(new NfeModel { GrvID = 543687 });

                //if (nfe == null)
                //{
                // var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal(594289, 1, TestSystemEnvironment.Production);
                var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal(grvId: 868480, usuarioId: 1, isDev: TestSystemEnvironment.Production);

                for (int i = 0; i < nfe.Count; i++)
                {
                    Console.WriteLine("MENSAGEM: " + nfe[i]);
                }
                
                //}

                //GlobalDataBaseController.ConnectDataBase();

                //ConfiguracoesController.id_usuario = 1;

                // new WsNfeController().EmitirNotaFiscal(543687, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Emissão da Nota Fiscal Eletrônica

            List<int> grvs = new List<int> { 578427, 626511 };

            #region Emissão da Nota Fiscal Eletrônica
            try
            {
                //foreach (var grv in grvs)
                //{
                //    try
                //    {
                //        var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //        (
                //            grvId: grv,
                //            usuarioId: 1,
                //            isDev: false
                //        );

                //        foreach (var res in result)
                //        {
                //            Console.WriteLine("JSON: " + res);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine("ERRO: " + ex.Message);
                //    }
                //}

                //var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //(
                //    grvId: 832359,
                //    usuarioId: 1,
                //    isDev: false
                //);

                //foreach (var res in result)
                //{
                //    Console.WriteLine("JSON: " + res);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Emissão da Nota Fiscal Eletrônica


            #region NOVA NF

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

                novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: 858422,
                    identificadorNota: 721190,
                    usuarioId: 1,
                    isDev: TestSystemEnvironment.Production
                );

                Console.WriteLine("MENSAGEM: " + novaNfe[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion NOVA NF


            // Recebimento
            try
            {
                var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 594289,
                    IdentificadorNota = 718711,
                    Homologacao = TestSystemEnvironment.Production,
                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 833723,
                    IdentificadorNota = 712035,
                    Homologacao = TestSystemEnvironment.Production,
                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 826282,
                    IdentificadorNota = 710414,
                    Homologacao = TestSystemEnvironment.Development,
                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                if (true)
                {

                }

                //try
                //{
                //    var imagem = new NfeImagemController().Selecionar(27114);

                //    File.WriteAllBytes(@"D:\Temp\RetornoRecortado.jpg", imagem.Imagem);
                //}
                //catch (Exception ex)
                //{

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            // Cancelamento
            try
            {
                var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                {
                    GrvId = 836989,
                    IdentificadorNota = 715109,
                    Justificativa = "TESTES",
                    Homologacao = TestSystemEnvironment.Development,
                    UsuarioId = 1
                });

                aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                {
                    GrvId = 543715,
                    IdentificadorNota = 700262,
                    Justificativa = "TESTES",
                    Homologacao = TestSystemEnvironment.Development,
                    UsuarioId = 1
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}