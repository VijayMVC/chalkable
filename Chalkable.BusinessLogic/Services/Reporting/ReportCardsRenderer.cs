using System;
using System.IO;
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

        public static byte[] RenderToPdf<T>(string jadeTpl, string css, Model<T> model, string basePath)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            basePath = "//local.chalkable.com:5001";
            var baseUrl = "file:///" + basePath;
            var json = JsonConvert.SerializeObject(model);
            var html = $@"
<html>
    <head>
        <meta charset=""UTF-8"">
        <title>{model.Title}</title>
        
        <link href=""{basePath}/bower_components/font-awesome/css/font-awesome.min.css"" type=""text/css"" rel=""stylesheet"" />
        <link href=""{basePath}/bower_components/bootstrap/dist/css/bootstrap.min.css"" type=""text/css"" rel=""stylesheet"" />
        <link href= ""{basePath}/bower_components /bootstrap/dist/css/bootstrap-theme.min.css"" type =""text/css"" rel =""stylesheet"" />
        
        <link href='https://fonts.googleapis.com/css?family=Ubuntu:400,400italic,500' rel='stylesheet' type='text/css'>
		<link href=""https://fonts.googleapis.com/css?family=Open+Sans:300italic,400italic,300,600,400,500"" rel=""stylesheet"" type=""text/css"" />
        
        <script type= ""text/javascript"" src = ""{basePath}/bower_components/accounting/accounting.min.js "" ></script>
        <script type= ""text/javascript"" src = ""{basePath}/bower_components/jquery/dist/jquery.min.js"" ></script>
        <script type= ""text/javascript"" src = ""{basePath}/bower_components/jade/jade.js"" ></script>
        
      
        <style type=""text/css"">{css}</style>
        
        <script type=""text/javascript"">
            var Model = {json}; 
        </script>
        
        <script type=""text/jade"" id=""template"">{jadeTpl}</script>
        
    </head>
    <body style=""width: 1200px"">
        <div id=""container"" class=""fluid-container"">
            <h1>Error rendering report</h1>
        </div>
        <script>(function(jade, $, model, accounting) {{
            var template = $('#template').text();
            try {{
                var html = jade.render(template, {{Model: model, accounting: accounting}});
            }} catch (e) {{
                html = '<h1>Error rendering report</h1><pre>' + e.message + '\n\n' + e.stack + '</pre>';
            }}
            $('#container').html(html);
        }})(jade, jQuery, Model, accounting);
        </script>
    </body>
</html>";
            return RenderToPdf(basePath, baseUrl, html);
        }

        public static byte[] RenderToPdf(string basePath, string baseUrl, string html)
        {
            var htmlToPdfConverter = new HtmlToPdfConverter
            {
                HtmlViewerWidth = 1200,
                ConversionDelay = 1,
                //LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=",
            };

            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
            htmlToPdfConverter.PdfDocumentOptions.TopMargin = 36;
            htmlToPdfConverter.PdfDocumentOptions.RightMargin = 36;
            htmlToPdfConverter.PdfDocumentOptions.BottomMargin = 36;
            htmlToPdfConverter.PdfDocumentOptions.LeftMargin = 36;
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;

            var footerHtmlWithPageNumbers = new HtmlToPdfVariableElement("Page &p; of &P;", baseUrl)
            {
                FitHeight = true
            };

            // Add variable HTML element with page numbering to footer
            htmlToPdfConverter.PdfFooterOptions.AddElement(footerHtmlWithPageNumbers);

            var oldCwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(basePath);
            try
            {

#if DEBUG
                using (var fileS = new FileStream(DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".html", FileMode.CreateNew))
                {
                    var htmlBytes = Encoding.UTF8.GetBytes(html);
                    fileS.Write(htmlBytes, 0, htmlBytes.Length);
                }
#endif

                var bytes = htmlToPdfConverter.ConvertHtml(html, baseUrl);

#if DEBUG
                using (var fileS = new FileStream(DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".pdf", FileMode.CreateNew))
                {
                    fileS.Write(bytes, 0, bytes.Length);
                }
#endif

                return bytes;
            }
            finally
            {
                Directory.SetCurrentDirectory(oldCwd);
            }
        }

        public static string ResolveAppDataPath(string path)
        {
            var appRoot = Environment.GetEnvironmentVariable("RoleRoot");
            appRoot = string.IsNullOrWhiteSpace(appRoot) ? Directory.GetCurrentDirectory() : Path.Combine(appRoot, "approot\\bin\\App_Data");

            return Path.Combine(appRoot, path);
        }
    }
}
