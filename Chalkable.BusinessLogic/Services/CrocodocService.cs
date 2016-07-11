using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Chalkable.BusinessLogic.Model;
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
        string GetToken();
        string GetStrorageUrl();
        string GetCrocodocApiUrl();
        string BuildDownloadDocumentUrl(string uuid, string docName);
        string BuildDownloadhumbnailUrl(string uuid, int docWith, int docHeigth);
        IList<CrocodocDocumentStatus> GetDocumentsStatus(IList<string> uuids);
        IList<CrocodocDocumentStatus> WaitForDocuments(IList<string> uuids);
    }

    public class CrocodocService : ICrocodocService
    {
        private const string TOKEN = "token";
        private const string UUID = "uuid";
        private const string ADMIN = "admin";
        private const string USER = "user";
        private const string EDITABLE = "editable";
        private const string SESSION_CREATE = "session/create";
        private const string DOCUMENT_UPLOAD = "document/upload";
        private const string DOCUMENT_STATUS = @"document/status";

        private const string PDF_EXT = ".pdf";
        private const string TRUE = "true";

        private const string CROCODOC_API_URL_FORMAT = "download/document?uuid={0}&pdf={1}&annotated={2}&token={3}";
        private const string CROCODOC_THUMBNAIL_URL_FORMAT = "download/thumbnail?token={0}&uuid={1}&size={2}x{3}";
        
        public DocumentUploadResponse UploadDocument(string fileName, byte[] fileContent)
        {
            if(!IsDocument(fileName))
                throw new ChalkableException("Current file is not a document");

            var res = UploadFileToCrocodoc(fileName, fileContent);
            if (res == null)
                throw new UploadToCrocodocFailedException(ChlkResources.ERR_PROCESSING_FILE);

            return res;
        }

        public StartSessionResponse StartViewSession(StartViewSessionRequestModel model)
        {
            var wc = new WebClient();
            var nameValue = new NameValueCollection
                {
                    {TOKEN, GetToken()},
                    {UUID, model.Uuid},
                    {EDITABLE, model.CanAnnotate.ToString().ToLower()},
                    {USER, string.Format("{0},{1}", model.PersonId, model.PersonName)},
                    {ADMIN, model.IsOwner.ToString().ToLower()}
                };
            var str = Encoding.ASCII.GetString(wc.UploadValues(UrlTools.UrlCombine(GetCrocodocApiUrl(), SESSION_CREATE), nameValue));
            return Deserialize<StartSessionResponse>(str);
        }

        public bool IsDocument(string fileName)
        {
            return MimeHelper.GetTypeByName(fileName) == MimeHelper.AttachmenType.Document;
        }

        public string GetToken()
        {
            return PreferenceService.Get(Preference.CROCODOC_TOKEN).Value;
        }

        public string GetStrorageUrl()
        {
            return PreferenceService.Get(Preference.CROCODOC_URL).Value;
        }

        public string GetCrocodocApiUrl()
        {
            return PreferenceService.Get(Preference.CROCODOC_API_URL).Value;
        }

        public string BuildDownloadDocumentUrl(string uuid, string docName)
        {
            if (!(CanBuildDownloadApiUrl(uuid))) return null;
            return string.Format(GetCrocodocApiUrl() + CROCODOC_API_URL_FORMAT, uuid, IsPdf(docName), TRUE, GetToken());
        }

        public string BuildDownloadhumbnailUrl(string uuid, int docWith, int docHeigth)
        {
            if (!(CanBuildDownloadApiUrl(uuid))) return null;
            return string.Format(GetCrocodocApiUrl() + CROCODOC_THUMBNAIL_URL_FORMAT, GetToken(), uuid, docWith, docHeigth);
        }

        private bool CanBuildDownloadApiUrl(string uuid)
        {
            return !string.IsNullOrEmpty(GetToken()) && !string.IsNullOrEmpty(GetStrorageUrl())
                && !string.IsNullOrEmpty(uuid);
        }

        private static bool IsPdf(string docName)
        {
            return PDF_EXT == (Path.GetExtension(docName) ?? string.Empty).ToLower();
        }
        private DocumentUploadResponse UploadFileToCrocodoc(string fileName, byte[] fileContent)
        {
            var nvc = new NameValueCollection { { TOKEN, GetToken() } };
            var fileType = MimeHelper.GetContentTypeByName(fileName);
            return ChalkableHttpFileLoader.HttpUploadFile(UrlTools.UrlCombine(GetCrocodocApiUrl(), DOCUMENT_UPLOAD)
                , fileName, fileContent, fileType, HandleUploadException, Deserialize<DocumentUploadResponse>, nvc);
        }


        private const string ERROR_FORMAT = "Error calling : '{0}' ;\n ErrorMessage : {1}";
        private void HandleUploadException(WebException ex)
        {
            var reader = new StreamReader(ex.Response.GetResponseStream());
            var msg = reader.ReadToEnd();
            var traceMsg = string.Format(ERROR_FORMAT, ex.Response.ResponseUri, msg);
            Trace.TraceError(traceMsg);
        }
        
        private T Deserialize<T>(string response)
        {
            return (new JsonSerializer()).Deserialize<T>(new JsonTextReader(new StringReader(response)));
        }

        public IList<CrocodocDocumentStatus> GetDocumentsStatus(IList<string> uuids)
        {
            var joinedIds = uuids.JoinString(",");
            var url = $@"{GetCrocodocApiUrl()}{DOCUMENT_STATUS}?{TOKEN}={GetToken()}&uuids={joinedIds}";
            var wc = new WebClient();

            var responce = Encoding.ASCII.GetString(wc.DownloadData(url));
            return JsonConvert.DeserializeObject<IList<DocumentStatusResponce>>(responce)
                .Select(CrocodocDocumentStatus.Create).ToList();
        }

        public IList<CrocodocDocumentStatus> WaitForDocuments(IList<string> uuids)
        {
            IList<CrocodocDocumentStatus> docsStates;
            while (true)
            {
                docsStates = GetDocumentsStatus(uuids);
                if (docsStates.Any(x => x.DocumentStatus == DocumentStatus.Processing || x.DocumentStatus == DocumentStatus.Queued))
                    Thread.Sleep(400);
                else
                    break;
            }

            return docsStates;
        }
    }

    public class DocumentStatusResponce
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("viewable")]
        public bool Viewable { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
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
