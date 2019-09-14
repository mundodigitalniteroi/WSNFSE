using NFSE.Domain.Entities;
using System;

namespace NFSE.TesteUsuario
{
    class Program
    {
        static void Main(string[] args)
        {
            string result = string.Empty;

            Console.WriteLine("TESTE DE SOLICITACAO DA NOTA FISCAL");

            #region Teste de Solicitação
            //var capaAutorizacaoNfse = new nfse.CapaAutorizacaoNfse
            //{
            //    Homologacao = true,
            //    IdentificadorNota = 123456,
            //    UsuarioId = 1,

            //    Autorizacao = new nfse.Autorizacao
            //    {
            //        data_emissao = "2019-08-28T04:34:06",
            //        natureza_operacao = "1",
            //        optante_simples_nacional = "false",

            //        prestador = new nfse.Prestador
            //        {
            //            cnpj = "08397160003658",
            //            codigo_municipio = "5103403",
            //            inscricao_municipal = "172692"
            //        },

            //        servico = new nfse.Servico
            //        {
            //            aliquota = "5.00",
            //            codigo_cnae = "5223100",
            //            codigo_tributario_municipio = "",
            //            discriminacao = @"ISS Tributado de acordo com a Lei Complementar Nº 460 de 22/10/2008 Processo Nº 9094604500 - Carga Tributária 18,45% fonte IBPT Serviços de Transporte/Remoção de Veículos",
            //            item_lista_servico = "1101",
            //            iss_retido = "false",
            //            valor_iss = "0.05",
            //            valor_servicos = "1.0"
            //        },

            //        tomador = new nfse.Tomador()
            //        {
            //            cpf = "07172853750",
            //            email = "cristineysoares@gmail.com",
            //            razao_social = "CRISTINEY SOARES",
            //            telefone = "2199999999",

            //            endereco = new nfse.Endereco
            //            {
            //                bairro = "Maria Paula",
            //                cep = "24756660",
            //                complemento = "Bloco 12 Apto 403",
            //                logradouro = "estrada da paciencia",
            //                numero = "2939",
            //                uf = "RJ"
            //            }
            //        }
            //    }
            //};

            //try
            //{
            //    using (var ws = new nfse.WSnfseSoapClient())
            //    {
            //        result = ws.SolicitarEmissaoNotaFiscal(capaAutorizacaoNfse);
            //    }

            //    Console.WriteLine(result + Environment.NewLine);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de Solicitação

            Console.WriteLine("TESTE DE RETORNO DA NOTA FISCAL");

            try
            {
                var retorno = new nfse.RetornoNotaFiscal();

                using (var ws = new nfse.WSnfseSoapClient())
                {
                    retorno = ws.ReceberNotaFiscal(new nfse.Consulta
                    {

                        IdentificadorNota = 700005,
                        CnpjPrestador = "08397160003658",
                        Homologacao = true,
                        UsuarioId = 1
                    });

                    Console.WriteLine(retorno.Status + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            Console.WriteLine("TESTE DE CANCELAMENTO DA NOTA FISCAL");

            try
            {
                using (var ws = new nfse.WSnfseSoapClient())
                {
                    result = ws.CancelarNotaFiscal(new nfse.Cancelamento
                    {
                        IdentificadorNota = 123456,
                        CnpjPrestador = "08397160003658",
                        Justificativa = "TESTE DE CANCELAMENTO",
                        Homologacao = true,
                        UsuarioId = 1
                    });

                    Console.WriteLine(result + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}