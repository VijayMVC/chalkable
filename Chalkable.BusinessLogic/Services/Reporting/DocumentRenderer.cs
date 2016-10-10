using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Winnovative;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public static class DocumentRenderer
    {
        private const string LicenseKey = "zUNQQlNXQlNaVlZCU1BMUkJRU0xTUExbW1tbQlI=";

        public static byte[] MergePdfDocuments(IList<byte[]> files)
        {
            var res = new Document {AutoCloseAppendedDocs = true, LicenseKey = LicenseKey};
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
            return MergePdfDocuments(files);
        }
        

        public static byte[] RenderToPdf(string basePath, string baseUrl, string html, string header, string footer)
        {
            baseUrl = baseUrl.StartsWith("//") ? ("https:" + baseUrl) : baseUrl;

            var htmlToPdfConverter = InitializeConverter();
            if (!string.IsNullOrWhiteSpace(header))
                AddHeader(htmlToPdfConverter, header, baseUrl);
            if (!string.IsNullOrWhiteSpace(footer))
                AddFooter(htmlToPdfConverter, footer, baseUrl);
            var bytes = htmlToPdfConverter.ConvertHtml(html, baseUrl);

            //Directory.SetCurrentDirectory(basePath);
            //var oldCwd = Directory.GetCurrentDirectory();
            //SaveView(basePath, html);
            //SaveView(basePath, header, "Header");
            //SaveView(basePath, footer, "Footer");
            //Directory.SetCurrentDirectory(oldCwd);

            return bytes;
        }

        private static HtmlToPdfConverter InitializeConverter()
        {
            var htmlToPdfConverter = new HtmlToPdfConverter
            {
                HtmlViewerWidth = 1200,
                ConversionDelay = 0,
                LicenseKey = LicenseKey
            };
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 18;
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 18;

            return htmlToPdfConverter;
        }

        private static void AddHeader(HtmlToPdfConverter converter, string htmlHeader, string baseUrl)
        {
            converter.PdfDocumentOptions.ShowHeader = true;
            var pdfElement = new HtmlToPdfElement(htmlHeader, baseUrl);
            converter.PdfHeaderOptions.AddElement(pdfElement);

            converter.PrepareRenderPdfPageEvent += e =>
            {
                e.Page.ShowHeader = e.IsNewPage;
            };
        }

        private static void AddFooter(HtmlToPdfConverter converter, string htmlFooter, string baseUrl)
        {

            converter.PdfDocumentOptions.ShowFooter = true;
            var footerHtmlWithPageNumbers = new HtmlToPdfVariableElement(htmlFooter, baseUrl)
            {
                FitHeight = true
            };
            // Add variable HTML element with page numbering to footer
            converter.PdfFooterOptions.AddElement(footerHtmlWithPageNumbers);
        }

        //this only for test 
        private static void SaveView(string path, string html, string namePrefix = null)
        {
            if(string.IsNullOrWhiteSpace(html)) return;

#if DEBUG
                using (var fileS = new FileStream((namePrefix ?? "") + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".html", FileMode.CreateNew))
                {
                    var htmlBytes = Encoding.UTF8.GetBytes(html);
                    fileS.Write(htmlBytes, 0, htmlBytes.Length);
                }
            
#endif
        }

        //this only for test 
        private static void SavePdf(string path, byte[] bytes, string html)
        {
            var oldCwd = Directory.GetCurrentDirectory();
            try
            {
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
