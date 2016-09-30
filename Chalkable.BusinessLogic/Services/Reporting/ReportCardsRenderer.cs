using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.Common.JsonContractTools;
using Chalkable.Data.Master.Model;
using Newtonsoft.Json;
using Winnovative;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public static class ReportCardsRenderer
    {
        public class Model
        {
            public string Title { get; set; }
            public Object Data { get; set; }
        }

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
        
        public static byte[] Render(string basePath, string baseUrl, CustomReportTemplate template, object dataSource)
        {
            var model = new Model {Data = dataSource};
            baseUrl = baseUrl.StartsWith("//") ? ("https:" + baseUrl) : baseUrl;
            var header = template.Header != null ? BuildView(baseUrl, template.Header, model) : null;
            var footer = template.Footer != null ? BuildView(baseUrl, template.Footer, model) : null;
            var bodyHtml = BuildView(baseUrl, template, model);
            return RenderToPdf(basePath, baseUrl, bodyHtml, header, footer);
        }

        private static string BuildView(string baseUrl, CustomReportTemplate template, object data)
        {
            var model = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            });
            return
                $@"
<!DOCTYPE html>
<html>
    <head>
            <title></title>
            <link href = ""http://fonts.googleapis.com/css?family=Pacifico"" rel= ""stylesheet"" type = ""text/css""/>
            <link href = ""http://fonts.googleapis.com/css?family=Oswald"" rel=""stylesheet"" type = ""text/css""/>
            <link href = ""http://fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,300,600,400,700"" rel = ""stylesheet"" type=""text/css""/>

            <link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap.min.css"" type = ""text/css"" rel = ""stylesheet""/>                 
            <link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap-theme.min.css"" type = ""text/css"" rel = ""stylesheet""/>                                                     
            <script src = ""{baseUrl}/app/bower/jquery/dist/jquery.js""></script>      
            <script src = ""{baseUrl}/app/bower/jade/jade.js"" ></script>

            <style type=""text/css"">{template.Style}</style>
            
            <script type=""text/javascript"">var Model = {model}</script>
            <script type=""text/jade"" id=""template"">{template.Layout}</script>

    </head>
    <body>
        <div id=""container"" class=""fluid-container"">
              <h1> Demo Report </h1>
        </div>    
         <script type=""text/javascript"">
            (function(jade, $, model) {{
                var template = $('#template').text();
                var html = """";
                try
                {{
                    html = jade.render(template, {{ model: model.data }});
                }}
                catch (e)
                {{
                    html = '<h1>Error rendering report</h1><pre>' + e.message + '\n\n' + e.stack + '</pre>';
                }}
            $('#container').html(html);
            }})(jade, jQuery, Model);
        </script>
    </body>
</html>";

        }


        private static string PrepareBaseUrl(string scripRoot)
        {
#if DEBUG
            return "https:" + scripRoot;
#else
            return scripRoot;
#endif
        }

        private static string BuildLinks(string scripRoot)
        {
            var baseUrl = PrepareBaseUrl(scripRoot);
#if DEBUG
            return $@"<link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap.min.css"" type = ""text/css"" rel = ""stylesheet""/>                 
            <link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap-theme.min.css"" type = ""text/css"" rel = ""stylesheet""/>                                                     
            <script src = ""{baseUrl}/app/bower/jquery/dist/jquery.js""></script>      
            <script src = ""{baseUrl}/app/bower/jade/jade.js"" ></script>";
#else
            return $@"";
#endif
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
            if(!string.IsNullOrWhiteSpace(header))
                AddHeader(htmlToPdfConverter, header, baseUrl);
            if(!string.IsNullOrWhiteSpace(footer))
                AddFooter(htmlToPdfConverter, footer, baseUrl);

            Directory.SetCurrentDirectory(basePath);
            var bytes = htmlToPdfConverter.ConvertHtml(html, baseUrl);
            //SaveViewAndPadf(basePath, bytes, html);
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
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;
            htmlToPdfConverter.PrepareRenderPdfPageEvent += e =>
            {
                e.Page.ShowHeader = e.IsNewPage;           
            };
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
