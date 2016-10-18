using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Chalkable.Common.JsonContractTools;
using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class TemplateRenderer : IDisposable
    {
        
        private readonly V8JsEngine JsEngine;
        private static readonly IList<string> scriptFileAddresses = new List<string>
        {
            @"Scripts\jade.js",
            @"Scripts\TemplateRenderer.js"
        }; 

        public TemplateRenderer(string baseServerPath)
        {
            JsEngine = new V8JsEngine();
            var contentToRun = new StringBuilder();
            FileStream fileStream = null;
            try
            {
                foreach (var fileAddress in scriptFileAddresses)
                {
                    var fullAddress = Path.Combine(baseServerPath, fileAddress);
                    fileStream = File.OpenRead(fullAddress);
                    using (var reader = new StreamReader(fileStream))
                    {
                        contentToRun.Append(reader.ReadToEnd());
                        contentToRun.Append(" ");
                    }
                }
                JsEngine.Execute(contentToRun.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        public string Render(string template, object data)
        {  
            var jsonData = JsonConvert.SerializeObject(new {Data = data}, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new LowercaseContractResolver()
            });
            var res = JsEngine.CallFunction<string>("RenderTemplate", jsonData, template);
            return res;
        }

        public void Dispose()
        {
            JsEngine.Dispose();
        }
    }
}