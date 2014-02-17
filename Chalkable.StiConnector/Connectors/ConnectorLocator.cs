﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorLocator
    {
        public const string REQ_ON_FORMAT = "Request on: {0}";
        public string BaseUrl { get; private set; }
        public string Token { get; private set; }
        public DateTime TokenExpires { get; private set; }

        public ConnectorLocator(string token, string baseUrl, DateTime tokenExpires)
        {
            Token = token;
            BaseUrl = baseUrl + "Api/";
            TokenExpires = tokenExpires;
            InitServices();
        }

        private void InitServices()
        {
            UsersConnector = new UsersConnector(this);
            SchoolConnector = new SchoolConnector(this);
            AcadSessionConnector = new AcadSessionConnector(this);
            StudentConnector = new StudentConnector(this);
            GradeLevelConnector = new GradeLevelConnector(this);
            GenderConnector = new GenderConnector(this);
            ContactConnector = new ContactConnector(this);
            AttendanceConnector = new AttendanceConnector(this);
            ActivityConnector = new ActivityConnector(this);
            ActivityAttachmentsConnector = new ActivityAttachmentsConnector(this);
            AttachmentConnector = new AttachmentConnector(this);
            SectionStandardConnector = new SectionStandardConnector(this);
            ActivityScoreConnector = new ActivityScoreConnector(this);
        }

        public UsersConnector UsersConnector { get; private set; }
        public SchoolConnector SchoolConnector { get; private set; }
        public AcadSessionConnector AcadSessionConnector { get; private set; }
        public StudentConnector StudentConnector { get; private set; }
        public GradeLevelConnector GradeLevelConnector { get; private set; }
        public GenderConnector GenderConnector { get; private set; }
        public ContactConnector ContactConnector { get; private set; }
        public AttendanceConnector AttendanceConnector { get; private set; }
        public ActivityConnector ActivityConnector { get; private set; }
        public ActivityAttachmentsConnector ActivityAttachmentsConnector { get; private set; }
        public AttachmentConnector AttachmentConnector { get; private set; }
        public SectionStandardConnector SectionStandardConnector { get; private set; }
        public ActivityScoreConnector ActivityScoreConnector { get; private set; }

        public class TokenModel
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public static string GetToken(string userName, string password, string baseUrl, out DateTime expires)
        {
            expires = DateTime.Now;
            var client = new WebClient();
            string credentials = string.Format("{0}:{1}", userName, password);
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string credentialsBase64 = Convert.ToBase64String(credentialsBytes);
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentialsBase64;
            client.Encoding = Encoding.UTF8;

            var url = string.Format("{0}Api/{1}", baseUrl, "token");
            Debug.WriteLine(REQ_ON_FORMAT, url);
            var x = typeof(TokenModel);
            var ser = new DataContractJsonSerializer(x);
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(client.DownloadData(url));
                var tm = (TokenModel)ser.ReadObject(stream);
                expires = expires.AddSeconds(tm.expires_in);
                return tm.access_token;
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
            finally
            {
                if(stream != null)
                    stream.Close();
            }
            //using (var stream = new MemoryStream(client.DownloadData(url)))
            //{
            //    var tm = (TokenModel)ser.ReadObject(stream);
            //    expires = expires.AddSeconds(tm.expires_in);
            //    return tm.access_token;
            //}
        }


        public static ConnectorLocator Create(string userName, string password, string baseUrl)
        {
            DateTime expires;
            var token = GetToken(userName, password, baseUrl, out expires);
            return new ConnectorLocator(token, baseUrl, expires);
        }
    }
}