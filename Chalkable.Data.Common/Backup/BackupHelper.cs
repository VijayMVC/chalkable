﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using System.Xml;
using Chalkable.Common;
using Chalkable.Data.Common.SqlAzure.ImportExport;
using Chalkable.Data.Common.Storage;

namespace Chalkable.Data.Common.Backup
{
    public class BackupHelper : BaseStorageHelper
    {
        public const string BACKUP_CONTAINER = "databasebackupcontainer";

        private static ConnectionInfo BuildConnectionInfo(string serverName, string databaseName)
        {
            return new ConnectionInfo
                {
                    ServerName = serverName,
                    DatabaseName = databaseName,
                    UserName = Settings.ChalkableSchoolDbUser,
                    Password = Settings.ChalkableSchoolDbPassword
                };
        }

        private const string BLOB_URL_TEMPLATE = "{0}{1}/{2}-{3}.bacpac";

        private static void PrepareBlob(long time, string databaseName, out string key, out string blobUri)
        {
            var cloudStorageAccount = GetDefaultStorageAccount();
            key = Convert.ToBase64String(cloudStorageAccount.Credentials.ExportKey());
            blobUri = String.Format(BLOB_URL_TEMPLATE, cloudStorageAccount.BlobEndpoint.AbsoluteUri, BACKUP_CONTAINER, time, databaseName);
        }

        private static ExportInput BuildExportInput(long time, string serverName, string databaseName)
        {
            string key, blobUri;
            PrepareBlob(time, databaseName, out key, out blobUri);
            var exportInputs = new ExportInput
            {
                
                
                BlobCredentials = new BlobStorageAccessKeyCredentials
                {
                    StorageAccessKey = key,
                    Uri = blobUri
                },
                ConnectionInfo = BuildConnectionInfo(serverName, databaseName)
            };
            return exportInputs;
        }

        private static ImportInput BuildImportInput(long time, string serverName, string databaseName)
        {
            string key, blobUri;
            PrepareBlob(time, databaseName, out key, out blobUri);
            var importInputs = new ImportInput
            {
                AzureEdition = "Web",
                DatabaseSizeInGB = 1,
                BlobCredentials = new BlobStorageAccessKeyCredentials
                {
                    StorageAccessKey = key,
                    Uri = blobUri
                },
                ConnectionInfo = BuildConnectionInfo(serverName, databaseName)
            };
            return importInputs;
        }

        private static string Endpoint
        {
            get
            {
                var endpoint = Settings.DbBackupServiceUrl;
                if (string.IsNullOrEmpty(endpoint))
                    throw new Exception("Db export endpoint is not configured");
                return endpoint;
            }
        }

        public static string DoExport(long time, string serverName, string databaseName)
        {
            var exportInputs = BuildExportInput(time, serverName, databaseName);

            WebRequest webRequest = WebRequest.Create(Endpoint + @"/Export");
            webRequest.Method = WebRequestMethods.Http.Post;
            webRequest.ContentType = @"application/xml";

            using (Stream webRequestStream = webRequest.GetRequestStream())
            {
                var dataContractSerializer = new DataContractSerializer(exportInputs.GetType());
                dataContractSerializer.WriteObject(webRequestStream, exportInputs);    
            }
            try
            {
                var webResponse = webRequest.GetResponse();
                using (var stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                        throw new Exception();
                    var xmlStreamReader = XmlReader.Create(stream);
                    xmlStreamReader.ReadToFollowing("guid");
                    string requestGuid = xmlStreamReader.ReadElementContentAsString();
                    return requestGuid;
                }
            }
            catch (WebException responseException)
            {
                Trace.WriteLine(string.Format("Request Falied:{0}", responseException.Message));
                if (responseException.Response != null)
                {
                    var statusCodeLine = string.Format("Status Code: {0}", ((HttpWebResponse) responseException.Response).StatusCode);
                    var messageLine = string.Format("Status Description: {0}\n\r", ((HttpWebResponse)responseException.Response).StatusDescription);
                    Trace.WriteLine(statusCodeLine);
                    Trace.WriteLine(messageLine);
                    throw new Exception(responseException.Message, new Exception(statusCodeLine + "\n" + messageLine));
                }
                throw;
            }
        }

        public static string DoImport(long time, string serverName, string databaseName)
        {
            var webRequest = WebRequest.Create(Endpoint + @"/Import");
            webRequest.Method = WebRequestMethods.Http.Post;
            webRequest.ContentType = @"application/xml";

            var importInputs = BuildImportInput(time, serverName, databaseName);

            using (var webRequestStream = webRequest.GetRequestStream())
            {
                var dataContractSerializer = new DataContractSerializer(importInputs.GetType());
                dataContractSerializer.WriteObject(webRequestStream, importInputs);
                webRequestStream.Close();
            }
            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
                using (var stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                        throw new Exception();
                    XmlReader xmlStreamReader = XmlReader.Create(stream);
                    xmlStreamReader.ReadToFollowing("guid");
                    var requestGuid = xmlStreamReader.ReadElementContentAsString();
                    return requestGuid;
                }
            }
            catch (WebException responseException)
            {
                Trace.WriteLine(string.Format("Request Falied:{0}", responseException.Message));
                if (responseException.Response != null)
                {
                    Trace.WriteLine(string.Format("Status Code: {0}", ((HttpWebResponse)responseException.Response).StatusCode));
                    Trace.WriteLine(string.Format("Status Description: {0}\n\r", ((HttpWebResponse)responseException.Response).StatusDescription));
                }
                throw;
            }
        }

        public static List<StatusInfo> CheckRequestStatus(string requestGuid, string serverName)
        {
            var webRequest = WebRequest.Create(Endpoint + string.Format("/Status?servername={0}&username={1}&password={2}&reqId={3}",
                    HttpUtility.UrlEncode(serverName),
                    HttpUtility.UrlEncode(Settings.ChalkableSchoolDbUser),
                    HttpUtility.UrlEncode(Settings.ChalkableSchoolDbPassword),
                    HttpUtility.UrlEncode(requestGuid)));

            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.ContentType = @"application/xml";
            var webResponse = webRequest.GetResponse();
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return null;
                var xmlStreamReader = XmlReader.Create(stream);
                var dataContractSerializer = new DataContractSerializer(typeof(List<StatusInfo>));
                return (List<StatusInfo>)dataContractSerializer.ReadObject(xmlStreamReader, true);   
            }
        }
    }
}
