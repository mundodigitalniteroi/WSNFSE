using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.NFe;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;

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

            //var grvs = new GrvController().Listar(new GrvEntity { NumeroFormularioGrv = "10151" });

            //int grvId = 0;

            //if (grvs != null)
            //{
            //    grvId = grvs[0].GrvId;

            //    if (grvs.Count > 1)
            //    {
            //        Debug.WriteLine($"FORAM ENCONTRADOS MAIS DE UM GRV");

            //        Console.WriteLine($"FORAM ENCONTRADOS MAIS DE UM GRV");

            //        Console.ReadLine();

            //        Environment.Exit(-1);
            //    }
            //}
            //else
            //{
            //    Debug.WriteLine($"NÚMERO DE GRV NÃO ENCONTRADO");

            //    Console.WriteLine($"NÚMERO DE GRV NÃO ENCONTRADO");

            //    Console.ReadLine();

            //    Environment.Exit(-1);
            //}

            //Debug.WriteLine($"GRV ID: {grvId}");

            //Console.WriteLine($"GRV ID: {grvId}");

            // NfeExcluirController.Excluir(749458);

            #region Teste de Solicitação Simplificado
            //try
            //{
            //    var nfe = new NfeGerarNotaFiscalController().GerarNotaFiscal
            //    (
            //        grvId: grvId,

            //        usuarioId: 1,

            //        isDev: IsTestEnvironment
            //    );

            //    for (int i = 0; i < nfe.Count; i++)
            //    {
            //        Console.WriteLine("MENSAGEM: " + nfe[i]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de Solicitação Simplificado


            #region Teste de Solicitação de uma nova NF
            //try
            //{
            //    var novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
            //    (
            //        grvId: grvId,

            //        identificadorNota: 754023,

            //        usuarioId: 1,

            //        isDev: IsTestEnvironment
            //    );

            //    Console.WriteLine("MENSAGEM: " + novaNfe[0]);

            //    novaNfe = new NfeGerarNotaFiscalController().GerarNovaNotaFiscal
            //    (
            //        grvId: grvId,

            //        identificadorNota: 753803,

            //        usuarioId: 1,

            //        isDev: IsTestEnvironment
            //    );

            //    Console.WriteLine("MENSAGEM: " + novaNfe[0]);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
            #endregion Teste de Solicitação de uma nova NF


            #region Teste de retorno da Nota Fiscal (Download da NF)
            //try
            //{
            //    var aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
            //    {
            //        GrvId = grvId,

            //        IdentificadorNota = 754053,

            //        Homologacao = IsTestEnvironment,

            //        UsuarioId = 1
            //    });

            //    Console.WriteLine("MENSAGEM: " + aux);

            //    //aux = new NfeReceberNotaFiscalController().ReceberNotaFiscal(new Consulta
            //    //{
            //    //    GrvId = grvId,

            //    //    IdentificadorNota = 751627,

            //    //    Homologacao = IsTestEnvironment,

            //    //    UsuarioId = 1
            //    //});

            //    //Console.WriteLine("MENSAGEM: " + aux);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("ERRO: " + ex.Message);
            //}
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

            var increment = 11;
            var dados = new NfseMassivoController().ListarDados();
            Console.WriteLine($"GERAÇÃO MASSIVA DE NFSE - {dados.Count} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            foreach (var dado in dados)
            {
                Console.WriteLine($"{increment} - Tomador: {dado.Nome}");

                var capaAutorizacaoNfse = new CapaAutorizacaoNfse
                {
                    IdentificadorNota = increment,

                    Autorizacao = new Autorizacao
                    {
                        data_emissao = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        natureza_operacao = "1",
                        optante_simples_nacional = "false",

                        prestador = new Prestador
                        {
                            cnpj = "34447382000118",
                            codigo_municipio = "2910800",
                            inscricao_municipal = "718556"
                        },

                        servico = new Servico
                        {
                            aliquota = "2.00",
                            codigo_cnae = "8299704",
                            codigo_tributario_municipio = "1712",
                            discriminacao = $@"Prestação de Serviços de Leiloeiro para alienação do bem:<br><br>  
                                            Lote {dado.Lote} - Leilão {dado.Leilao} - Status: {dado.Status}<br>
                                            Placa: {dado.Placa} - UF: {dado.UfVeiculo} - Chassi: {dado.Chassi} - Renavam: {dado.Renavam}<br>
                                            MarcaModelo: {dado.MarcaModelo} - COR: {dado.Cor} - COMBUSTÍVEL: {dado.Combustivel}<br>
                                            ANO FAB: {dado.AnoFab} - ANO MOD: {dado.AnoMod} - NUMERO MOTOR: {dado.NumeroMotor}<br>
                                            DATA APREENSAO: {dado.DataApreensao}<br><br>",
                            item_lista_servico = "1712",
                            //iss_retido = dado.CpfCnpj.Length > 11 ? "true" : "false",
                            valor_servicos = dado.ValorServico.ToString().Replace(",", ".")
                        },

                        tomador = new Tomador()
                        {
                            cpf = dado.CpfCnpj.Length <= 11 ? dado.CpfCnpj : null,
                            cnpj = dado.CpfCnpj.Length > 11 ? dado.CpfCnpj : null,
                            email = dado.Email,
                            razao_social = dado.Nome,
                            telefone = dado.Telefone,                            
                             
                            endereco = new Endereco
                            {
                                bairro = dado.Bairro,
                                cep = dado.Cep.ToString(),
                                complemento = dado.Complemento,
                                logradouro = dado.Logradouro,
                                numero = dado.Numero,
                                uf = dado.Uf,
                                codigo_municipio = dado.CpfCnpj.Length > 11 ? dado.CodigoMunicipioIbge : null
                            }
                        }
                    }
                };

                try
                {
                    var json = new NfeSolicitarEmissaoNotaFiscalController().SolicitarEmissaoNotaFiscalAvulso(capaAutorizacaoNfse);

                    var nfe =  new JavaScriptSerializer()
                    {
                        MaxJsonLength = int.MaxValue
                    }.Deserialize<RetornoNotaFiscalEntity>(json);

                    var nfeEntity = new NfeEntity()
                    {
                        IdentificadorNota = capaAutorizacaoNfse.IdentificadorNota,
                        Cnpj = capaAutorizacaoNfse.Autorizacao.prestador.cnpj,
                        StatusNfe = nfe.status
                    };

                    new NfseMassivoController().Cadastrar(nfeEntity);

                    dado.NotaCriada = true;
                    dado.IdentificadorNota = capaAutorizacaoNfse.IdentificadorNota;
                    new NfseMassivoController().AtualizarDados(dado);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO: " + ex.Message);
                }

                increment++;
            }

            Console.WriteLine($"FINALIZADO AS {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            Console.ReadLine();
        }
    }
}