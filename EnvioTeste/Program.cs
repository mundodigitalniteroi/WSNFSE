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
                599219,
                822930,
                823570,
                823780,
                824164,
                824270
            };


            #region Emissão da Nota Fiscal Eletrônica
            try
            {
                //foreach (var grvId in lista)
                //{
                //    var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //    (
                //        grvId: grvId,
                //        usuarioId: 1,
                //        isDev: false
                //    );

                //    foreach (var item in result)
                //    {
                //        Console.WriteLine("JSON: " + item);
                //    }
                //}

                var grv = new GrvController().Selecionar("14059590");

                int grvId = grv.GrvId;

                var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                (
                    grvId: grv.GrvId,
                    usuarioId: 1,
                    isDev: true
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