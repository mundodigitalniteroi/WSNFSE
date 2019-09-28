using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.NFe;
using System;
using System.IO;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            // Solicitação
            var capaAutorizacaoNfse = new CapaAutorizacaoNfse
            {
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

            int grvId = 0;

            int[] lista =
            {
                543713
            };

            #region Emissão da Nota Fiscal Eletrônica
            foreach (var item in lista)
            {
                //Console.WriteLine("GRV ID: " + item);

                try
                {
                    //var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                    //(
                    //    grvId: 637990,
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
            }

            try
            {
                //var result = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                //(
                //    grvId: 21255,
                //    faturamentoServicoTipoVeiculoId: 937,
                //    usuarioId: 1,
                //    isDev: true
                //);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            try
            {
                //var grv = new GrvController().Selecionar("14059599");

                //grvId = grv.GrvId;

                //grvId = 584327;

                //var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //(
                //    grvId: grvId,
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
                    GrvId = 824331,
                    IdentificadorNota = 709297,
                    Homologacao = false,
                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                try
                {
                    var imagem = new NfeImagemController().Selecionar(27114);

                    File.WriteAllBytes(@"D:\Temp\RetornoRecortado.jpg", imagem.Imagem);
                }
                catch (Exception ex)
                {

                }

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 823740,
                    IdentificadorNota = 709296,
                    Homologacao = false,
                    UsuarioId = 1
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                if (true)
                {

                }

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = 543707,
                //    IdentificadorNota = 700218,
                //    Homologacao = true,
                //    UsuarioId = 1
                //});

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = 543702,
                //    IdentificadorNota = 700193,
                //    Homologacao = true,
                //    UsuarioId = 1
                //});

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = 543702,
                //    IdentificadorNota = 700193,
                //    Homologacao = true,
                //    UsuarioId = 1
                //});

                //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                //{
                //    GrvId = 543702,
                //    IdentificadorNota = 700193,
                //    Homologacao = true,
                //    UsuarioId = 1
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            // Cancelamento
            try
            {
                //var aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                //{
                //    GrvId = 824360,
                //    IdentificadorNota = 700143,
                //    Justificativa = "TESTES",
                //    Homologacao = true,
                //    UsuarioId = 1
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}