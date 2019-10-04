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


            #region Emissão da Nota Fiscal Eletrônica
            try
            {
                //var result = new NfeGerarNotaFiscalController().GerarNotaFiscal
                //(
                //    grvId: 830128,
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
                    grvId: 543725,
                    identificadorNota: 700301,
                    faturamentoServicoTipoVeiculoId: 16349,
                    usuarioId: 1,
                    isDev: true
                );

                Console.WriteLine("MENSAGEM: " + novaNfe[0]);

                novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
                (
                    grvId: 543725,
                    identificadorNota: 700302,
                    faturamentoServicoTipoVeiculoId: 281,
                    usuarioId: 1,
                    isDev: true
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
                    GrvId = 543725,
                    IdentificadorNota = 700301,
                    Homologacao = true,
                    UsuarioId = 1,
                    BaixarImagemOriginal = false
                });

                Console.WriteLine("MENSAGEM: " + aux.status);

                aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
                {
                    GrvId = 543725,
                    IdentificadorNota = 700302,
                    Homologacao = true,
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
                    GrvId = 543715,
                    IdentificadorNota = 700261,
                    Justificativa = "TESTES",
                    Homologacao = true,
                    UsuarioId = 1
                });

                aux = new NfeCancelamentoController().CancelarNotaFiscal(new Cancelamento
                {
                    GrvId = 543715,
                    IdentificadorNota = 700262,
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