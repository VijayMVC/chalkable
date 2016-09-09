using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Winnovative;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public static class ReportCardsRenderer
    {
        public class Model<T>
        {
            public string Title { get; set; }
            public T Data { get; set; }
        }

        public static byte[] MargePdfDocuments(IList<byte[]> files)
        {
            var res = new Document {AutoCloseAppendedDocs = true};
            foreach (var file in files)
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(file, 0, file.Length);
                    res.AppendDocument(new Document(stream));
                }
            }
            return res.Save();
        }

        public static byte[] RenderToPdf(string basePath, string baseUrl, IList<string> htmls, string header, string footer)
        {
            var files = htmls.Select(html => RenderToPdf(basePath, baseUrl, html, header, footer)).ToList();
            return MargePdfDocuments(files);
        }

        public static byte[] RenderToPdf(string basePath, string baseUrl, string html, string header, string footer)
        {
            baseUrl = baseUrl.StartsWith("//") ? ("https:" + baseUrl) : baseUrl;

            var htmlToPdfConverter = InitializeConverter();
            AddHeader(htmlToPdfConverter, header, baseUrl);
            AddFooter(htmlToPdfConverter, footer, baseUrl);

            Directory.SetCurrentDirectory(basePath);
            var bytes = htmlToPdfConverter.ConvertHtml(html, baseUrl);
            SaveViewAndPadf(basePath, bytes, html);
            return bytes;
        }

        private static HtmlToPdfConverter InitializeConverter()
        {
            var htmlToPdfConverter = new HtmlToPdfConverter
            {
                HtmlViewerWidth = 1200,
                ConversionDelay = 1,
                //LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=",
            };
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;
            htmlToPdfConverter.PrepareRenderPdfPageEvent += e => { e.Page.ShowHeader = e.PageNumber > 1; };
            return htmlToPdfConverter;
        }

        private static void AddHeader(HtmlToPdfConverter converter, string htmlHeader, string baseUrl)
        {
            var pdfElement = new HtmlToPdfElement(htmlHeader, baseUrl);
            converter.PdfHeaderOptions.AddElement(pdfElement);
        }

        private static void AddFooter(HtmlToPdfConverter converter, string htmlFooter, string baseUrl)
        {
            var footerHtmlWithPageNumbers = new HtmlToPdfVariableElement(htmlFooter, baseUrl)
            {
                FitHeight = true
            };
            // Add variable HTML element with page numbering to footer
            converter.PdfFooterOptions.AddElement(footerHtmlWithPageNumbers);
        }
        
        //this only for test 
        private static void SaveViewAndPadf(string path, byte[] bytes, string html)
        {
            var oldCwd = Directory.GetCurrentDirectory();
            try
            {
#if DEBUG
                using (var fileS = new FileStream(DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".html", FileMode.CreateNew))
                {
                    var htmlBytes = Encoding.UTF8.GetBytes(html);
                    fileS.Write(htmlBytes, 0, htmlBytes.Length);
                }
#endif
#if DEBUG
                using (var fileS = new FileStream(DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".pdf", FileMode.CreateNew))
                {
                    fileS.Write(bytes, 0, bytes.Length);
                }
#endif
            }
            finally
            {
                Directory.SetCurrentDirectory(oldCwd);
            }
        }
    }
}
