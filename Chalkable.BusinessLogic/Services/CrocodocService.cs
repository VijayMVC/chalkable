using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Master.Model;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services
{
    public interface ICrocodocService
    {
        DocumentUploadResponse UploadDocument(string fileName, byte[] fileContent);
        StartSessionResponse StartViewSession(StartViewSessionRequestModel model);
        bool IsDocument(string fileName);
    }

    public class CrocodocService : ICrocodocService
    {
        private const string TOKEN = "token";
        private const string UUID = "uuid";
        private const string ADMIN = "ADMIN";
        private const string USER = "USER";
        private const string EDITABLE = "EDITABLE";
        private const string SESSION_CREATE = "session/create";
        private const string DOCUMENT_UPLOAD = "document/upload";

        public DocumentUploadResponse UploadDocument(string fileName, byte[] fileContent)
        {
            if(!IsDocument(fileName))
                throw new ChalkableException("Current file is not a document");

            var res = UploadFileToCrocodoc(fileName, fileContent);
            if (res == null)
                throw new ChalkableException(ChlkResources.ERR_PROCESSING_FILE);
            return res;
        }

        public StartSessionResponse StartViewSession(StartViewSessionRequestModel model)
        {
            var wc = new WebClient();
            var nameValue = new NameValueCollection
                {
                    {TOKEN, Token},
                    {UUID, model.Uuid},
                    {EDITABLE, model.CanAnnotate.ToString().ToLower()},
                    {USER, string.Format("{0},{1}", model.PersonId, model.PersonName)},
                    {ADMIN, model.IsOwner.ToString().ToLower()}
                };
            var str = Encoding.ASCII.GetString(wc.UploadValues(UrlTools.UrlCombine(StrorageUrl, SESSION_CREATE), nameValue));
            return Deserialize<StartSessionResponse>(str);
        }

        public bool IsDocument(string fileName)
        {
            return MimeHelper.GetTypeByName(fileName) == MimeHelper.AttachmenType.Document;
        }

        private DocumentUploadResponse UploadFileToCrocodoc(string fileName, byte[] fileContent)
        {
            var nvc = new NameValueCollection { { TOKEN, Token } };
            var fileType = MimeHelper.GetContentTypeByName(fileName);
            return ChalkableHttpFileLoader.HttpUploadFile(UrlTools.UrlCombine(CrocodocApiUrl, DOCUMENT_UPLOAD)
                , fileName, fileContent, fileType, null, Deserialize<DocumentUploadResponse>, nvc);
        }

        private T Deserialize<T>(string response)
        {
            return (new JsonSerializer()).Deserialize<T>(new JsonTextReader(new StringReader(response)));
        }

        protected string Token
        {
            get { return PreferenceService.Get(Preference.CROCODOC_TOKEN).Value; }
        }
        protected string StrorageUrl
        {
            get { return PreferenceService.Get(Preference.CROCODOC_URL).Value; }
        }
        protected string CrocodocApiUrl
        {
            get { return PreferenceService.Get(Preference.CROCODOC_API_URL).Value; }
        }
        
    }


    public class DocumentUploadResponse
    {
        public string uuid { get; set; }
    }
    public class StartSessionResponse
    {
        public string session { get; set; }
    }

    public class StartViewSessionRequestModel
    {
        public string Uuid { get; set; }
        public bool IsOwner { get; set; }
        public bool CanAnnotate { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }
}
