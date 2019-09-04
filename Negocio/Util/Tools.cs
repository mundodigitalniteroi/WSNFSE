using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Negocio.Util
{
    public class Tools
    {
        public string PostNfse(string uri, string json, string token)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "POST";

            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":" + ""));
            httpWebRequest.Headers.Add("Authorization", "Basic " + base64String);
            httpWebRequest.PreAuthenticate = true;

            byte[] bytes = new UTF8Encoding().GetBytes(json);

            httpWebRequest.ContentLength = (long)bytes.Length;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
                requestStream.Write(bytes, 0, bytes.Length);

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        return streamReader.ReadToEnd().Trim();
                }
            }
            catch (WebException ex)
            {
                return new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }
        }

        public string CancelarNfse(string uri, string json, string token)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "DELETE";

            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":" + ""));

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
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
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

        public string ObjToJSON(object obj)
        {
            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Serialize(obj);
        }

        public object JSONToObj(string serial, Type tipo)
        {
            return new JavaScriptSerializer()
            {
                MaxJsonLength = int.MaxValue
            }.Deserialize(serial, tipo);
        }

        public string ToCsv(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (dataTable.Columns.Count == 0)
                return (string)null;

            foreach (object column in (InternalDataCollectionBase)dataTable.Columns)
            {
                if (column == null)
                    stringBuilder.Append(";");
                else
                    stringBuilder.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\";");
            }

            stringBuilder.Replace(";", Environment.NewLine, stringBuilder.Length - 1, 1);

            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
            {
                foreach (object obj in row.ItemArray)
                {
                    if (obj == null)
                        stringBuilder.Append(";");
                    else
                        stringBuilder.Append("\"" + obj.ToString().Replace("\"", "\"\"") + "\";");
                }

                stringBuilder.Replace(";", Environment.NewLine, stringBuilder.Length - 1, 1);
            }

            return stringBuilder.ToString();
        }

        public Bitmap ObterImagemEndereco(string url)
        {
            Bitmap Retorno = null;

            var thread = new Thread(() => Retorno = new Bitmap(CaptureWebPage(url)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            while (thread.IsAlive)
            {
                if (stopwatch.Elapsed.Seconds > 180)
                {
                    thread.Abort();
                }
            }

            return Retorno;
        }

        public Bitmap CaptureWebPage(string URL)
        {
            WebBrowser webBrowser = new WebBrowser
            {
                ScrollBarsEnabled = false,
                ScriptErrorsSuppressed = true
            };

            webBrowser.Navigate(URL);

            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Thread.Sleep(2500);

            int width = 900;

            int height = 1100; // webBrowser.Document.Body.ScrollRectangle.Height + 50;

            webBrowser.Width = width;
            webBrowser.Height = height;

            Bitmap bitmap = new Bitmap(width, height);

            webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));

            return bitmap;
        }

        internal string GetNfse(string uri, string token)
        {
            string base64String = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(token + ":" + ""));

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.Method = "GET";

            httpWebRequest.Headers.Add("Authorization", "Basic " + base64String);

            httpWebRequest.PreAuthenticate = true;

            var utF8Encoding = new UTF8Encoding();

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

        private CredentialCache GetCredential(string url)
        {
            return new CredentialCache()
            {
                {
                    new Uri(url),
                    "Basic",
                    new NetworkCredential("2D6xPXxoXRyIuTyUjS6HbiLao7Xr50Mb", "")
                }
            };
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
    }
}