using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace NFSE.Business.Util
{
    public class Tools
    {
        public static string PostNfse(string uri, string json, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":" + ""));

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("Authorization", "Basic " + base64String);

            httpWebRequest.PreAuthenticate = true;

            byte[] bytes = new UTF8Encoding().GetBytes(json);

            httpWebRequest.ContentLength = (long)bytes.Length;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd().Trim();
                    }
                }
            }
            catch (WebException ex)
            {
                return new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }
        }

        public static string CancelarNfse(string uri, string json, string token)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":"));

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "DELETE";

            httpWebRequest.Headers.Add("Authorization", "Basic " + base64String);

            httpWebRequest.PreAuthenticate = true;

            byte[] bytes = new UTF8Encoding().GetBytes(json);

            httpWebRequest.ContentLength = (long)bytes.Length;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd().Trim();
                    }
                }
            }
            catch (WebException ex)
            {
                return new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }
        }

        public static string ObjToJSON(object obj)
        {
            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
                
            }.Serialize(obj);
        }

        public static object JSONToObj(string serial, Type tipo)
        {
            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize(serial, tipo);
        }

        public static string ToCsv(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (dataTable.Columns.Count == 0)
            {
                return (string)null;
            }

            foreach (object column in (InternalDataCollectionBase)dataTable.Columns)
            {
                if (column == null)
                {
                    stringBuilder.Append(";");
                }
                else
                {
                    stringBuilder.Append('\"').Append(column.ToString().Replace("\"", "\"\"")).Append("\";");
                }
            }

            stringBuilder.Replace(";", Environment.NewLine, stringBuilder.Length - 1, 1);

            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
            {
                foreach (object obj in row.ItemArray)
                {
                    if (obj == null)
                    {
                        stringBuilder.Append(";");
                    }
                    else
                    {
                        stringBuilder.Append('\"').Append(obj.ToString().Replace("\"", "\"\"")).Append("\";");
                    }
                }

                stringBuilder.Replace(";", Environment.NewLine, stringBuilder.Length - 1, 1);
            }

            return stringBuilder.ToString();
        }

        public static Bitmap ObterImagemEndereco(string url)
        {
            Bitmap Retorno = null;

            Thread thread = new Thread(() => Retorno = new Bitmap(CaptureWebPage(url)));

            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            while (thread.IsAlive)
            {
                if (stopwatch.Elapsed.TotalSeconds > 180)
                {
                    thread.Abort();

                    throw new Exception($"A tentativa de download da imagem superou {stopwatch.Elapsed.Minutes} e foi abortada pelo WebService");
                }
            }

            stopwatch.Stop();

            return Retorno;
        }

        public static Bitmap CaptureWebPage(string url)
        {
            using (var webBrowser = new WebBrowser { ScrollBarsEnabled = false, ScriptErrorsSuppressed = true })
            {
                webBrowser.Navigate(url);

                // Já tentei isso, não funciona
                // webBrowser.ScriptErrorsSuppressed = false;

                while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }

                // TODO: Desabilitar a linha abaixo
                // Thread.Sleep(2500);

                const int width = 1500;

                const int height = 1700; // webBrowser.Document.Body.ScrollRectangle.Height + 50;

                webBrowser.Width = width;

                webBrowser.Height = height;

                Bitmap bitmap = new Bitmap(width, height);

                webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));

                return bitmap;
            }
        }

        public static Bitmap CaptureWebPageNew(string url)
        {
            var navigator = new InternetService();

            navigator.Navigate(url);

            return null;

            //using (var webBrowser = new WebBrowser { ScrollBarsEnabled = false, ScriptErrorsSuppressed = true })
            //{
            //    webBrowser.Navigate(url);

            //    // Já tentei isso, não funciona
            //    // webBrowser.ScriptErrorsSuppressed = false;

            //    while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            //    {
            //        Application.DoEvents();
            //    }

            //    // TODO: Desabilitar a linha abaixo
            //    // Thread.Sleep(2500);

            //    const int width = 1500;

            //    const int height = 1700; // webBrowser.Document.Body.ScrollRectangle.Height + 50;

            //    webBrowser.Width = width;

            //    webBrowser.Height = height;

            //    Bitmap bitmap = new Bitmap(width, height);

            //    webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));

            //    return bitmap;
            //}
        }

        internal string GetNfse(string uri, string token)
        {
            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":" + ""));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "GET";

            httpWebRequest.Headers.Add("Authorization", "Basic " + base64String);

            httpWebRequest.PreAuthenticate = true;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd().Trim();
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
        }

        public string DataTableToJSON(DataTable table)
        {
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();

            foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                foreach (DataColumn column in (InternalDataCollectionBase)table.Columns)
                {
                    dictionary[column.ColumnName] = (object)Convert.ToString(row[column]);
                }

                dictionaryList.Add(dictionary);
            }

            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Serialize((object)dictionaryList);
        }

        public string DriveToSave()
        {
            return @"C:\";
        }

        public void GravarLog(string message)
        {
            string drive = new Tools().DriveToSave();

            if (!Directory.Exists($@"F:\Logs\NfeCadastroNotaFiscal"))
            {
                Directory.CreateDirectory($@"F:\Logs\NfeCadastroNotaFiscal");
            }

            using (StreamWriter sw = new StreamWriter($@"F:\Logs\NfeCadastroNotaFiscal\NfeGerarNotaFiscalController.log", true, Encoding.UTF8))
            {
                sw.WriteLine(message);
            }
        }
    }
}