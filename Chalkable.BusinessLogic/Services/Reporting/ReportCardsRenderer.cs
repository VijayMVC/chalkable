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
        
        public static byte[] Render(string basePath, string baseUrl, CustomReportTemplate template, object dataSource)
        {
            baseUrl = baseUrl.StartsWith("//") ? ("https:" + baseUrl) : baseUrl;
            var header = template.Header != null ? BuildView(baseUrl, template.Header, dataSource) : null;
            var footer = template.Footer != null ? BuildView(baseUrl, template.Footer, dataSource) : null;
            var bodyHtml = BuildView(baseUrl, template, dataSource);
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
            <link href = ""http://fonts.googleapis.com/css?family=Pacifico"" rel= ""stylesheet"" type = ""text/css"" />
            <link href = ""http://fonts.googleapis.com/css?family=Oswald"" rel=""stylesheet"" type = ""text/css"" />
            <link href = ""http://fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,300,600,400,700"" rel = ""stylesheet"" type=""text/css"" />
            <link href = ""{baseUrl}/app/bower/chosen/chosen.min.css"" rel = ""stylesheet"" type = ""text/css"" />
            <link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap.min.css"" type = ""text/css"" rel = ""stylesheet"" />                 
            <link href = ""{baseUrl}/app/bower/bootstrap/dist/css/bootstrap-theme.min.css"" type = ""text/css"" rel = ""stylesheet"" />
            <link href = ""{baseUrl}/app/bower/jquery-ui/themes/smoothness/jquery-ui.css"" rel = ""stylesheet"" type = ""text/css"" />                                              
            <link href = ""{baseUrl}/app/jquery/snippet/jquery.snippet.min.css"" rel = ""stylesheet"" type = ""text/css"" />
                                                       

            <script src = ""{baseUrl}/app/bower/jquery/dist/jquery.js"" ></ script >      
            <script src = ""{baseUrl}/app/bower/jade/jade.js"" ></ script >
            <style type = ""text/css"" >{template.Style}</style>

            <script type=""text/javascript"">{model}</script>
            <script type=""text/jade"" id=""template"">{template.Layout}</script>

    </head>
    <body>
        <div id=""container"" class=""fluid-container"">
              <h1> Demo Report </h1>
     
             </div>
     
         <script type=""text/javascript"" >
              var msg = "";
            if (!jade)
                msg = msg + ""jade wasn't download"";
            else
                msg = msg + "" jade has donwloaded"";

            if (!jQuery)
                msg = msg + "" jQuery wasn't download"";
            else
                msg = msg + "" jQuery wasn't download"";

            document.getElementById(""container"").innerHTML = ""<h1>"" + msg + ""</h1>"";
            try
            {{
                (function(jade, $, model) {{
                    var template = $('#template').text();
                    var html = "";
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
            }}
            catch (e)
            {{
                var error = '<h1>Error rendering report ....</h1><pre>' + e.message + '\n\n' + e.stack + '</pre>';
                jQuery('#container').html(error);
            }}
        </ script >
        </ body >
    </ html >";

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
            if(!string.IsNullOrWhiteSpace(header))
                AddHeader(htmlToPdfConverter, header, baseUrl);
            if(!string.IsNullOrWhiteSpace(footer))
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
