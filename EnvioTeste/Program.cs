using Negocio;
using NFSE.Domain.Entities;
using System;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {
            var capaAutorizacaoNfse = new nfse.CapaAutorizacaoNfse
            {
                Homologacao = true,
                CodigoRetorno = 700067,

                Autorizacao = new nfse.Autorizacao
                {
                    data_emissao = "2019-08-28T04:34:06",
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
                        discriminacao = @"ISS Tributado de acordo com a Lei Complementar Nº 460 de 22/10/2008 Processo Nº 9094604500 - Carga Tributária 18,45% fonte IBPT Serviços de Transporte/Remoção de Veículos",
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
                string result = string.Empty;

                using (var ws = new nfse.WSnfseSoapClient())
                {
                    result = ws.SolicitarEmissaoNotaFiscal(capaAutorizacaoNfse);
                }

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            try
            {
                var aux = new ControlarEnvio().ReceberNotaFiscal(new Consulta
                {
                    CodigoRetorno = 700069,
                    CnpjPrestador = "08397160003658",
                    Homologacao = true,
                    UsuarioId = 1
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            try
            {
                var aux = new ControlarEnvio().CancelarNotaFiscal(new Cancelamento
                {
                    CodigoRetorno = 700069,
                    CnpjPrestador = "08397160003658",
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