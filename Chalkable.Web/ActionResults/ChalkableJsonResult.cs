using System;
using System.Web;
using System.Web.Mvc;
using Chalkable.Common.Web;

namespace Chalkable.Web.ActionResults
{
    public class ChalkableJsonResult : JsonResult
    {
        private bool hideSensitive;
        public ChalkableJsonResult(bool hideSensitive)
        {
            this.hideSensitive = hideSensitive;
            ContentType = "application/json";
        }

        public int SerializationDepth
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = ContentType;
            if (Data != null)
            {
                var serializer = new MagicJsonSerializer(hideSensitive);
                serializer.MaxDepth = SerializationDepth;
                string result = serializer.Serialize(Data);
                response.Write(result);
            }

        }
    }
}