using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnvioTeste
{
    internal static class Program
    {
        private static void Main()
        {
            // DataBase.SystemEnvironment = SystemEnvironment.Development;

            DataBase.SystemEnvironment = SystemEnvironment.Production;

            bool isDevelopment = DataBase.SystemEnvironment.Equals(SystemEnvironment.Development);

            //int grvId = 1124013;

            //GrvEntity grv;

            //string[] grvs =
            //{
            //    "904112292"
            //};

            //List<GrvEntity> resultado;

            //for (int i = 0; i < grvs.Length; i++)
            //{
            //    resultado = new GrvController().Listar(new GrvEntity { NumeroFormularioGrv = grvs[i]/*, ClienteId = 41*/ });

            //    if (resultado != null)
            //    {
            //        if (resultado.Count > 1)
            //        {
            //            Console.WriteLine("GRV DUPLICADO: " + grvs[i] + ". PLACAS: ");

            //            foreach (var res in resultado)
            //            {
            //                Console.WriteLine(res.Placa);
            //            }

            //            continue;
            //        }

            //        SolicitarNotaFiscal(resultado[0].GrvId, isDevelopment);

            //        Console.WriteLine("GRV PROCESSADO: " + grvs[i]);
            //    }
            //    else
            //    {
            //        Console.WriteLine("GRV INEXISTENTE: " + grvs[i]);
            //    }
            //}

            //if (true)
            //{
            //    grv = new GrvController().Selecionar("912870087");

            //    if (grv != null)
            //    {
            //        grvId = grv.GrvId;
            //    }
            //}

            //const string identificadorNota = "865401";

            //SolicitarNotaFiscal(grvId, isDevelopment);

            // SolicitarNovaNotaFiscal(1012553, "899284", isDevelopment);

            //ReceberNotaFiscal(grvId, identificadorNota, isDevelopment);

            var list = new List<Nota>
            {
                new Nota { Grv = 1318298, Identificador = "865402" },
                new Nota { Grv = 1317405, Identificador = "865407" },
                new Nota { Grv = 1313865, Identificador = "865409" },
                new Nota { Grv = 1317199, Identificador = "865408" },
                new Nota { Grv = 1318156, Identificador = "865410" },
                new Nota { Grv = 1318414, Identificador = "865411" },
                new Nota { Grv = 1316177, Identificador = "865422" },
                new Nota { Grv = 1317033, Identificador = "865416" },
                new Nota { Grv = 1317264, Identificador = "865418" },
                new Nota { Grv = 1318177, Identificador = "865421" },
                new Nota { Grv = 1318660, Identificador = "865413" },
                new Nota { Grv = 1319195, Identificador = "865417" },
                new Nota { Grv = 1319383, Identificador = "865420" },
                new Nota { Grv = 1319520, Identificador = "865419" },
                new Nota { Grv = 1314972, Identificador = "865439" },
                new Nota { Grv = 1315352, Identificador = "865428" },
                new Nota { Grv = 1316983, Identificador = "865434" },
                new Nota { Grv = 1317152, Identificador = "865430" },
                new Nota { Grv = 1318382, Identificador = "865427" },
                new Nota { Grv = 1318569, Identificador = "865431" },
                new Nota { Grv = 1318654, Identificador = "865425" },
                new Nota { Grv = 1318669, Identificador = "865438" },
                new Nota { Grv = 1318820, Identificador = "865423" },
                new Nota { Grv = 1319148, Identificador = "865437" },
                new Nota { Grv = 1319208, Identificador = "865436" },
                new Nota { Grv = 1319242, Identificador = "865435" },
                new Nota { Grv = 1319245, Identificador = "865433" },
                new Nota { Grv = 1319262, Identificador = "865426" },
                new Nota { Grv = 1319540, Identificador = "865440" },
                new Nota { Grv = 1319550, Identificador = "865429" },
                new Nota { Grv = 1319118, Identificador = "865441" },
                new Nota { Grv = 1317420, Identificador = "865444" },
                new Nota { Grv = 1318516, Identificador = "865443" },
                new Nota { Grv = 1318841, Identificador = "865442" },
                new Nota { Grv = 1318956, Identificador = "865458" },
                new Nota { Grv = 1319114, Identificador = "865470" },
                new Nota { Grv = 1318558, Identificador = "865464" },
                new Nota { Grv = 1317738, Identificador = "865453" },
                new Nota { Grv = 1317740, Identificador = "865468" },
                new Nota { Grv = 1318099, Identificador = "865473" },
                new Nota { Grv = 1318344, Identificador = "865446" },
                new Nota { Grv = 1318368, Identificador = "865445" },
                new Nota { Grv = 1317249, Identificador = "865485" },
                new Nota { Grv = 1319144, Identificador = "865479" },
                new Nota { Grv = 1319146, Identificador = "865483" },
                new Nota { Grv = 1319192, Identificador = "865489" },
                new Nota { Grv = 1319214, Identificador = "865477" },
                new Nota { Grv = 1319219, Identificador = "865493" },
                new Nota { Grv = 1319122, Identificador = "865462" },
                new Nota { Grv = 1319124, Identificador = "865456" },
                new Nota { Grv = 1319386, Identificador = "865457" },
                new Nota { Grv = 1319510, Identificador = "865450" },
                new Nota { Grv = 1319515, Identificador = "865467" },
                new Nota { Grv = 1319611, Identificador = "865447" },
                new Nota { Grv = 1319693, Identificador = "865471" },
                new Nota { Grv = 1319694, Identificador = "865469" },
                new Nota { Grv = 1319695, Identificador = "865463" },
                new Nota { Grv = 1319715, Identificador = "865465" },
                new Nota { Grv = 1319719, Identificador = "865449" },
                new Nota { Grv = 1319729, Identificador = "865451" },
                new Nota { Grv = 1307320, Identificador = "865482" },
                new Nota { Grv = 1312757, Identificador = "865494" },
                new Nota { Grv = 1314061, Identificador = "865490" },
                new Nota { Grv = 1315065, Identificador = "865481" },
                new Nota { Grv = 1317006, Identificador = "865475" },
                new Nota { Grv = 1311331, Identificador = "865452" },
                new Nota { Grv = 1314657, Identificador = "865461" },
                new Nota { Grv = 1315027, Identificador = "865454" },
                new Nota { Grv = 1315106, Identificador = "865474" },
                new Nota { Grv = 1315119, Identificador = "865466" },
                new Nota { Grv = 1317093, Identificador = "865459" },
                new Nota { Grv = 1317104, Identificador = "865448" },
                new Nota { Grv = 1317251, Identificador = "865472" },
                new Nota { Grv = 1319236, Identificador = "865480" },
                new Nota { Grv = 1319289, Identificador = "865487" },
                new Nota { Grv = 1319519, Identificador = "865484" },
                new Nota { Grv = 1319521, Identificador = "865476" },
                new Nota { Grv = 1319530, Identificador = "865478" },
                new Nota { Grv = 1319704, Identificador = "865492" },
                new Nota { Grv = 1319707, Identificador = "865486" },
                new Nota { Grv = 1319815, Identificador = "865488" },
                new Nota { Grv = 1319820, Identificador = "865491" },
                new Nota { Grv = 1319293, Identificador = "865495" },
                new Nota { Grv = 1319508, Identificador = "865498" },
                new Nota { Grv = 1317028, Identificador = "865497" },
                new Nota { Grv = 1319235, Identificador = "865500" },
                new Nota { Grv = 1319151, Identificador = "865501" },
                new Nota { Grv = 1318350, Identificador = "865496" },
                new Nota { Grv = 1319128, Identificador = "865499" }
            };

            foreach (var nota in list)
            {
                CancelarNotaFiscal(nota.Grv, nota.Identificador, isDevelopment);
            }
            //CancelarNotaFiscal(1314842, identificadorNota, isDevelopment);
            Console.WriteLine("CONCLUIDO");
            Console.ReadLine();
        }

        #region Teste de solicitação de Nota Fiscal
        private static void SolicitarNotaFiscal(int grvId, bool isDevelopment)
        {
            try
            {
                var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
                (
                    grvId: grvId,

                    usuarioId: 1,

                    isDev: isDevelopment,

                    forcarGeracaoNfe: true
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
        }
        #endregion Teste de solicitação da Nota Fiscal

        #region Teste de solicitação de Nova Nota Fiscal
        private static void SolicitarNovaNotaFiscal(int grvId, string identificadorNota, bool isDevelopment)
        {
            try
            {
                var nfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: grvId,

                    identificadorNota: identificadorNota,

                    usuarioId: 1,

                    isDev: isDevelopment
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
        }
        #endregion Teste de solicitação da Nova Nota Fiscal

        #region Teste de retorno da Nota Fiscal (Download da NF)
        private static void ReceberNotaFiscal(int grvId, string identificadorNota, bool isDevelopment)
        {
            try
            {
                var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = grvId,

                    IdentificadorNota = identificadorNota,

                    Homologacao = isDevelopment,

                    UsuarioId = 1,

                    BaixarImagemOriginal = false
                });

                Console.WriteLine("MENSAGEM: " + aux);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
        }
        #endregion Teste de retorno da Nota Fiscal (Download da NF)

        #region Teste de cancelamento da Nota Fiscal
        private static void CancelarNotaFiscal(int grvId, string identificadorNota, bool isDevelopment)
        {
            try
            {
                var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                {
                    GrvId = grvId,

                    IdentificadorNota = identificadorNota,

                    Justificativa = "Emissão duplicada",

                    Homologacao = isDevelopment,

                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
        }
        #endregion Teste de cancelamento da Nota Fiscal

        private static void Processamentos(bool isDevelopment, int grvId, int identificadorNota = 0)
        {
            //DataBase.DisconnectDataBase();

            //if (DataBase.SystemEnvironment == SystemEnvironment.Development)
            //{
            //    DataBase.ConnectDataBase("Data Source=20.236.82.177;Initial Catalog=dbMobLinkDepositoPublicoDesenvolvimento;Persist Security Info=True;User ID=dp_user_dev;Password=5y3d#%&&!x");
            //}
            //else
            //{
            //    DataBase.ConnectDataBase("Data Source=20.236.82.177;Initial Catalog=dbMobLinkDepositoPublicoProducao;Persist Security Info=True;User ID=dp_user_prd;Password=4y3d#%&&!x");
            //}

            // DataBase.ConnectDataBase();

            // var grvs = new GrvController().Listar(new GrvEntity { StatusOperacaoId = 'E', ClienteId = 49 });

            // 943312
            // 877968

            var grvs = new GrvController().Listar(new GrvEntity { NumeroFormularioGrv = "900691870" });

            foreach (var grv in grvs)
            {
                try
                {
                    var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
                    (
                        grvId: grv.GrvId,

                        usuarioId: 1,

                        isDev: isDevelopment
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
            }

            if (grvs != null)
            {
                grvId = grvs[0].GrvId;

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

            Debug.WriteLine($"GRV ID: {grvId}");

            Console.WriteLine($"GRV ID: {grvId}");

            // NfeExcluirController.Excluir(749458);

            grvId = 949073;

            #region Teste de Solicitação Simplificado
            try
            {
                var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
                (
                    grvId: grvId,

                    usuarioId: 1,

                    isDev: isDevelopment
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


            grvId = 949073;

            #region Teste de Solicitação de uma nova NF
            try
            {
                var novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: grvId,

                    identificadorNota: "801814",

                    usuarioId: 1,

                    isDev: isDevelopment
                );

                Console.WriteLine("MENSAGEM: " + novaNfe[0]);

                //novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                //(
                //    grvId: grvId,

                //    identificadorNota: 753803,

                //    usuarioId: 1,

                //    isDev: IsTestEnvironment
                //);

                //Console.WriteLine("MENSAGEM: " + novaNfe[0]);
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

                    IdentificadorNota = "762905",

                    Homologacao = isDevelopment,

                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux);

                if (true)
                {

                }

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = grvId,

                //    IdentificadorNota = "751627",

                //    Homologacao = IsTestEnvironment,

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
            //try
            //{
            //    var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
            //    {
            //        GrvId = grvId,

            //        IdentificadorNota = "",

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
            // using NFSE.Domain.Entities.NFe;
            var capaAutorizacaoNfse = new CapaAutorizacaoNfse
            {
                IdentificadorNota = "",

                Autorizacao = new Autorizacao
                {
                    data_emissao = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    natureza_operacao = 1,
                    optante_simples_nacional = false,

                    prestador = new Prestador
                    {
                        cnpj = "08397160003658",
                        codigo_municipio = "5103403",
                        inscricao_municipal = "172692"
                    },

                    servico = new Servico
                    {
                        aliquota = "5.00",
                        codigo_cnae = "5223100",
                        codigo_tributario_municipio = "",
                        discriminacao = "ISS Tributado de acordo com a Lei Complementar Nº 460 de 22/10/2008 Processo Nº 9094604500 - Carga Tributária 18,45% fonte IBPT Serviços de Transporte/Remoção de Veículos",
                        item_lista_servico = "1101",
                        valor_iss = "0.05",
                        valor_servicos = "1.0"
                    },

                    tomador = new Tomador()
                    {
                        cpf = "07172853750",
                        email = "cristiney@gmail.com",
                        razao_social = "CRISTINEY SOARES",
                        telefone = "2199999999",

                        endereco = new Endereco
                        {
                            bairro = "Maria Paula",
                            cep = "24756660",
                            complemento = "Bloco 12 Apto 403",
                            logradouro = "estrada da paciencia",
                            numero = "2939",
                            uf = "RJ"
                        }
                    }
                }
            };

            try
            {
                var nfe = new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscalAvulso(capaAutorizacaoNfse);

                // Console.WriteLine(result + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
        }

    }

    public class Nota
    {
        public int Grv { get; set; }
        public string Identificador { get; set; }
    }
}