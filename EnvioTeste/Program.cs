using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.NFe;
using System;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            // Solicitação
            var capaAutorizacaoNfse = new CapaAutorizacaoNfse
            {
                Homologacao = true,
                IdentificadorNota = 700074,
                UsuarioId = 1,

                Autorizacao = new Autorizacao
                {
                    data_emissao = "2019-08-28T04:34:06",
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
                        discriminacao = @"ISS Tributado de acordo com a Lei Complementar Nº 460 de 22/10/2008 Processo Nº 9094604500 - Carga Tributária 18,45% fonte IBPT Serviços de Transporte/Remoção de Veículos",
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

            //try
            //{
            //    string result = string.Empty;

            //    using (var ws = new nfse.WSnfseSoapClient())
            //    {
            //        result = ws.SolicitarEmissaoNotaFiscal(capaAutorizacaoNfse);
            //    }

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
                //    new WsNfeController().EmitirNotaFiscal(543687, false);
                //}

                //GlobalDataBaseController.ConnectDataBase();

                //ConfiguracoesController.id_usuario = 1;

                //new WsNfeController().EmitirNotaFiscal(543687, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Emissão da Nota Fiscal Eletrônica

            int[] lista =
            {
                822988, 822991, 823001,
                823003, 823004, 823007, 823009, 823011, 823012, 823013, 823015, 823027, 823028,
                823029, 823031, 823032, 823033, 823037, 823049, 823050, 823053, 823054, 823082,
                823083, 823085, 823087, 823090, 823096, 823544, 823545, 823546, 823566, 823567,
                823569, 823571, 823589, 823596, 823602, 823604, 823605, 823606, 823608, 823617,
                823618, 823623, 823625, 823629, 823641, 823649, 823657, 823672, 823695, 823706,
                823710, 823713, 823714, 823715, 823721, 823735, 823736, 823740, 823742, 823744,
                823745, 823752, 823756, 823758, 823759, 823760, 823767, 823775, 823776, 823777,
                823781, 823789, 823793, 823796, 823828, 823831, 823832, 823833, 823837, 823838,
                823880, 823919, 823920, 823928, 823930, 823933, 823935, 823936, 823939, 823942,
                823944, 823947, 823949, 823950, 823956, 823966, 824173, 824175, 824177, 824178,
                824180, 824183, 824187, 824189, 824191, 824223, 824235, 824236, 824237, 824238,
                824241, 824242, 824247, 824248, 824249, 824253, 824254, 824255, 824257, 824259,
                824260, 824264, 824265, 824281, 824282, 824283, 824284, 824287, 824328, 824329,
                824330, 824331, 824332, 824334, 824335, 824345, 824346, 824348, 824351, 824353,
                824354, 824357, 824358, 824360, 824361, 824365, 824366, 824367, 824378, 824382,
                824383, 824384, 824391, 824392, 824407, 824408, 824409, 824414, 824415, 824416,
                824456, 824457, 824464, 824468, 824488, 824494, 824667, 824668, 824670, 824672,
                824680, 824681, 824682, 824684, 824690, 824691, 824695, 824698, 824700, 824701,
                824702
            };

            #region Emissão da Nota Fiscal Eletrônica
            foreach (var grvId in lista)
            {
                Console.WriteLine("GRV ID: " + grvId);

                try
                {
                    var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                    (
                        grvId: grvId,
                        usuarioId: 1,
                        isDev: false
                    );

                    foreach (var item in result)
                    {
                        Console.WriteLine("JSON: " + item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO: " + ex.Message);
                }
            }



            try
            {
                //var grv = new GrvController().Selecionar("14059590");

                //int grvId = grv.GrvId;

                //var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //(
                //    grvId: grv.GrvId,
                //    usuarioId: 1,
                //    isDev: true
                //);

                //foreach (var item in result)
                //{
                //    Console.WriteLine("JSON: " + item);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Emissão da Nota Fiscal Eletrônica

            // Recebimento
            try
            {
                var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 823578,
                    IdentificadorNota = 708677,
                    CnpjPrestador = "25329339000248",
                    Homologacao = false,
                    UsuarioId = 1
                });

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 823578,
                    IdentificadorNota = 708678,
                    CnpjPrestador = "25329339000248",
                    Homologacao = false,
                    UsuarioId = 1
                });
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
                    GrvId = 824360,
                    IdentificadorNota = 700143,
                    CnpjPrestador = "16952840000194",
                    Justificativa = "TESTES",
                    Homologacao = true,
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