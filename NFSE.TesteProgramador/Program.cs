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
            DataBase.SystemEnvironment = SystemEnvironment.Production;

            DataBase.ConnectDataBase();

            var grv = new GrvController().Selecionar("909030375");

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

            List<int> grvs = new List<int> { 578427, 626511, 636825, 636825, 824486, 576387, 576387, 822432, 822432, 831331, 831351, 831543, 831568, 831571, 832336, 832394, 832409, 832446, 828354, 828354
                                           , 830241, 830241, 830385, 830385, 830711, 830711, 831113, 831113, 831130, 831130, 831143, 831143, 831156, 831156, 831372, 831372, 831374, 831374, 831533, 831533
                                           , 831534, 831534, 831613, 831613, 831617, 831617, 831669, 831669, 831707, 831707, 831708, 831708, 832408, 832408, 832455, 832455, 617653, 617653, 641339, 641339
                                           , 822403, 822403, 827827, 827827, 828944, 828944, 829523, 829523, 831191, 831191, 831262, 831262, 831345, 831345, 831952, 831952, 831966, 831966, 831972, 831972
                                           , 831978, 831978, 831983, 831983, 831987, 831987, 831991, 831991, 832101, 832101, 832291, 832291, 828866, 828866, 828866, 830657, 830657, 830657, 831432, 831432
                                           , 831783, 831783, 832320, 832320, 828108, 828108, 828108, 831303, 831303, 831303, 831410, 831410, 831410, 831423, 831423, 831423, 831623, 831623, 831623, 832287
                                           , 832287, 832287, 832339, 832339, 832339, 832402, 832402, 832402, 830788, 830788, 830788, 831189, 831189, 831189, 831333, 831333, 831333, 832266, 832266, 832266
                                           , 832305, 832305, 832305, 832356, 832356, 832356, 832363, 832363, 829751, 829751, 829911, 829911, 829911, 830716, 830716, 830749, 830749, 830782, 830782, 830782
                                           , 831027, 831027, 831121, 831121, 831634, 831634, 831634, 831640, 831640, 831640, 831641, 831641, 831641, 832209, 832209};

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
                    grvId: 868447,
                    identificadorNota: 52858,
                    usuarioId: 1,
                    isDev: TestSystemEnvironment.Production
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