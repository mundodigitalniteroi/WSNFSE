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
    internal static class Program
    {
        private static void Main()
        {
            DataBase.SystemEnvironment = SystemEnvironment.Production;

            // DataBase.SystemEnvironment = SystemEnvironment.Development;

            bool IsTestEnvironment = DataBase.SystemEnvironment.Equals(SystemEnvironment.Development);

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

            var grvs = new GrvController().Listar(new GrvEntity { NumeroFormularioGrv = "10151" });

            int grvId = 0;

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

                    identificadorNota: 754023,

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

                    IdentificadorNota = 754053,

                    Homologacao = IsTestEnvironment,

                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux);

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = grvId,

                //    IdentificadorNota = 751627,

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
            // using NFSE.Domain.Entities.NFe;
            var capaAutorizacaoNfse = new CapaAutorizacaoNfse
            {
                IdentificadorNota = 0,

                Autorizacao = new Autorizacao
                {
                    data_emissao = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    natureza_operacao = "1",
                    optante_simples_nacional = "false",

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
                        iss_retido = "false",
                        valor_iss = "0.05",
                        valor_servicos = "1.0"
                    },

                    tomador = new Tomador()
                    {
                        cpf = "07172853750",
                        email = "cristineysoares@gmail.com",
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

            Console.ReadLine();
        }
    }
}