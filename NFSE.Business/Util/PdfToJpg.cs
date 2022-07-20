using Spire.Pdf;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace NFSE.Business.Util
{
    public abstract class PdfToJpg
    {
        public static void Process(string pdfFileName, string jpgFileName)
        {
            if (File.Exists(jpgFileName))
            {
                File.Delete(jpgFileName);
            }

            using (PdfDocument pdfDocument = new PdfDocument())
            {
                pdfDocument.LoadFromFile(pdfFileName);

                using (Image image = pdfDocument.SaveAsImage(0, 100, 100))
                {
                    image.Save(jpgFileName, ImageFormat.Jpeg);
                }
            }
        }
    }
}