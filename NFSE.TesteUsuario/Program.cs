using System;

namespace NFSE.TesteUsuario
{
    internal static class Program
    {
        private static void Main()
        {
            string result = string.Empty;

            #region Teste de Solicitação

            Console.WriteLine("SIMULAÇÃO DA NOTA FISCAL");

            var capaAutorizacaoNfse = new nfse.CapaAutorizacaoNfse
            {
                IdentificadorNota = 0,

                Autorizacao = new nfse.Autorizacao
                {
                    data_emissao = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    natureza_operacao = "1",
                    optante_simples_nacional = "false",

                    prestador = new nfse.Prestador
                    {
                        cnpj = "08397160003658",
                        codigo_municipio = "5103403",
                        inscricao_municipal = "172692"
                    },

                    servico = new nfse.Servico
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

                    tomador = new nfse.Tomador()
                    {
                        cpf = "07172853750",
                        email = "cristineysoares@gmail.com",
                        razao_social = "CRISTINEY SOARES",
                        telefone = "2199999999",

                        endereco = new nfse.Endereco
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
                using (var ws = new nfse.WSnfseSoapClient())
                {
                    result = ws.SolicitarEmissaoNotaFiscalAvulso(capaAutorizacaoNfse);
                }

                Console.WriteLine(result + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de Solicitação



            #region Teste de retorno da Nota Fiscal (Download da NF)

            Console.WriteLine("TESTE DE RETORNO DA NOTA FISCAL (DOWNLOAD DA NF)");

            try
            {
                var aux = new nfse.WSnfseSoapClient().ReceberNotaFiscalAvulso(new nfse.Consulta
                {
                    IdentificadorNota = 700155,
                    Homologacao = true,
                    UsuarioId = 1,
                    Cnpj = "08397160003658"
                });

                Console.WriteLine("MENSAGEM: " + aux.Html);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }
            #endregion Teste de retorno da Nota Fiscal (Download da NF)



            #region Teste de cancelamento da Nota Fiscal

            Console.WriteLine("TESTE DE CANCELAMENTO DA NOTA FISCAL");

            try
            {
                using (var ws = new nfse.WSnfseSoapClient())
                {
                    result = ws.CancelarNotaFiscalAvulso(new nfse.Cancelamento
                    {
                        IdentificadorNota = 000000,
                        Justificativa = "TESTE DE CANCELAMENTO",
                        Homologacao = true,
                        UsuarioId = 1,
                        Cnpj = "08397160003658"
                    });

                    Console.WriteLine(result + Environment.NewLine);
                }
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