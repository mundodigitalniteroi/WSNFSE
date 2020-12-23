using NFSE.Business.Tabelas.DP;
using NFSE.Business.Tabelas.Global;
using NFSE.Business.Util;
using NFSE.Domain.Entities.DP;
using NFSE.Domain.Entities.Global;
using NFSE.Domain.Entities.NFe;
using NFSE.Domain.Enum;
using NFSE.Infra.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace NFSE.Business.Tabelas.NFe
{
    public class NfseMassivoReceberNotaFiscalController
    {
        private void AtualizarNotaFiscal(NfeEntity nfe)
        {
            nfe.Status = 'E';

            new NfseMassivoController().Atualizar(nfe);
        }

        public RetornoNotaFiscalEntity ReceberNotaFiscal(Consulta model)
        {
            DataBase.SystemEnvironment = model.Homologacao ? SystemEnvironment.Development : SystemEnvironment.Production;

            NfeEntity nfe = new NfseMassivoController().ConsultarNotaFiscal(model.IdentificadorNota);

            model.NfeId = nfe.NfeId;

            #region Token
            NfePrestadorAvulsoEntity PrestadorAvulso;

            if ((PrestadorAvulso = NfePrestadorAvulsoPersistence.Selecionar(new NfePrestadorAvulsoEntity { Cnpj = model.Cnpj })) == null)
            {
                throw new Exception("Prestador não encontrado");
            }
            else if (PrestadorAvulso.Token == null)
            {
                throw new Exception("Prestador não possui Token configurado");
            }

            #endregion Empresa

            string json;

        Outer:

            try
            {
                json = new Tools().GetNfse(new NfeConfiguracao().GetRemoteServer() + "/" + model.IdentificadorNota, PrestadorAvulso.Token);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("por minuto"))
                {
                    Console.WriteLine("LIMITE DE REQUISIÇÕES POR MINUTO, AGUARDANDO 1 Min PARA CONTINUAR...");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    goto Outer;
                }

                AtualizarNotaFiscal(nfe);

                throw new Exception("Ocorreu um erro ao receber a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }

            try
            {
                return ProcessarRetorno(nfe, model, json);
            }
            catch (Exception ex)
            {
                AtualizarNotaFiscal(nfe);

                throw new Exception("Ocorreu um erro ao cadastrar a Nota Fiscal (" + model.IdentificadorNota + "): " + ex.Message);
            }
        }

        private RetornoNotaFiscalEntity ProcessarRetorno(NfeEntity nfe, Consulta identificaoNotaFiscal, string retornoJson)
        {
            RetornoNotaFiscalEntity retornoConsulta = new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize<RetornoNotaFiscalEntity>(retornoJson);

            if (retornoConsulta.status.Trim().Equals("processando_autorizacao", StringComparison.CurrentCultureIgnoreCase))
            {
                return retornoConsulta;
            }

            if (retornoConsulta.erros != null)
            {
                nfe.Status = 'E';

                new NfseMassivoController().Atualizar(nfe);

                return retornoConsulta;
            }

            if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
            {
                retornoConsulta.url = retornoConsulta.url.Replace("nfse.aspx", "/NFSE/contribuinte/notaprintimg.aspx");

                if (!string.IsNullOrWhiteSpace(retornoConsulta.url))
                {
                    retornoConsulta.Html = BaixarImagem(nfe.IdentificadorNota, retornoConsulta.url);
                }

                if (identificaoNotaFiscal.BaixarImagemOriginal)
                {
                    return retornoConsulta;
                }

                nfe.Status = nfe.Status == 'A' ? 'P' : 'T';

                new NfseMassivoController().AtualizarRetornoNotaFiscal(nfe, retornoConsulta);
            }

            return retornoConsulta;
        }

        private string BaixarImagem(int identificadorNota, string url)
        {
            string directory = @"D:\Sistemas\GeradorNF\NFE\" + DataBase.SystemEnvironment.ToString() + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd") + "\\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string str1 = directory + identificadorNota.ToString() + "Original.html";

            if (File.Exists(str1))
            {
                File.Delete(str1);
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", "Mob-Link");

                webClient.DownloadFile(url, str1);
            };

            return str1;
        }
    }
}