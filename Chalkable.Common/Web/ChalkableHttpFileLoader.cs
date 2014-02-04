using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common.Web
{
    public class ChalkableHttpFileLoader
    {
        public const string BOUNDARY_CONTENT_TYPE_FMT = "multipart/form-data; boundary={0}";
        public const string FORM_ITEM_CONTENT_DISPOSITION = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        public const string HEADER_CONTENT_DISPOSITION = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

        private const string FILE_CONTENT_TYPE = "file";


        private static void AddHeaders(WebRequest request, ICollection<KeyValuePair<string, string>> headers)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (headers == null || headers.Count <= 0) return;
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        private static HttpWebRequest InitWebRequest(string url, string file, byte[] fileContent, 
            string contentType, NameValueCollection nvc, IDictionary<string, string> headers, HttpMethod httpMethod)
        {
            string boundary = "----" + DateTime.UtcNow.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var wr = (HttpWebRequest)WebRequest.Create(url);
            AddHeaders(wr, headers);
            wr.ContentType = string.Format(BOUNDARY_CONTENT_TYPE_FMT, boundary);
            wr.Method = httpMethod.Method;
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();
            if (nvc != null)
            {
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(FORM_ITEM_CONTENT_DISPOSITION, key, nvc[key]);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string header = string.Format(HEADER_CONTENT_DISPOSITION, FILE_CONTENT_TYPE, file, contentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            Stream fileStream = new MemoryStream(fileContent);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            return wr;
        }

        public static T HttpUploadFile<T>(string url, string file, byte[] fileContent, 
            string contentType, Action<WebException> exAction, Func<WebResponse, T> resultAction, NameValueCollection nvc = null
            , IDictionary<string, string> headers = null, HttpMethod httpMethod = null)
        {
            httpMethod = httpMethod ?? HttpMethod.Post;
            var wr = InitWebRequest(url, file, fileContent, contentType, nvc, headers, httpMethod);
            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                return resultAction(wresp);
            }
            catch (WebException ex)
            {
                if(exAction != null)
                    exAction(ex);
            }
            finally
            {
               if (wresp != null)
                   wresp.Close();
            }
            return default(T);
        }

        public static T HttpUploadFile<T>(string url, string file, byte[] fileContent, string contentType
            , Action<WebException> exAction, Func<string, T> deserializeAction, NameValueCollection nvc)
        {
            return HttpUploadFile(url, file, fileContent, contentType
                , exAction, response => GetUploadFileResult(response, deserializeAction), nvc);
        }

        public static T HttpUploadFile<T>(string url, string file, byte[] fileContent, string contentType
            ,Action<WebException> exAction, Func<string, T> deserializeAction, NameValueCollection nvc
            , IDictionary<string, string> headers, HttpMethod method)
        {
            return HttpUploadFile(url, file, fileContent, contentType
                , exAction, response => GetUploadFileResult(response, deserializeAction), nvc, headers, method);
        }


        private static T GetUploadFileResult<T>(WebResponse response, Func<string, T> deserializeAction)
        {
            Stream stream2 = response.GetResponseStream();
            if (stream2 != null)
            {
                var reader2 = new StreamReader(stream2);
                string resp = reader2.ReadToEnd();
                return deserializeAction(resp);
            }
            return default(T);
        }
        
    }
}
