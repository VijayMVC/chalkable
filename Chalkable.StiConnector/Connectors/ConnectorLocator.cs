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

        public ConnectorLocator(string token, string baseUrl)
        {
            Token = token;
            BaseUrl = baseUrl;
            InitServices();
        }

        private void InitServices()
        {
            SchoolConnector = new SchoolConnector(this);
            AcadSessionConnector = new AcadSessionConnector(this);
            StudentConnector = new StudentConnector(this);
            GradeLevelConnector = new GradeLevelConnector(this);
            GenderConnector = new GenderConnector(this);
            ContactConnector = new ContactConnector(this);
            AttendanceConnector = new AttendanceConnector(this);
        }

        public SchoolConnector SchoolConnector { get; private set; }
        public AcadSessionConnector AcadSessionConnector { get; private set; }
        public StudentConnector StudentConnector { get; private set; }
        public GradeLevelConnector GradeLevelConnector { get; private set; }
        public GenderConnector GenderConnector { get; private set; }
        public ContactConnector ContactConnector { get; private set; }
        public AttendanceConnector AttendanceConnector { get; set; }

        public class TokenModel
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public static string GetToken(string userName, string password, string baseUrl)
        {
            
            var client = new WebClient();
            string credentials = string.Format("{0}:{1}", userName, password);
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string credentialsBase64 = Convert.ToBase64String(credentialsBytes);
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentialsBase64;
            client.Encoding = Encoding.UTF8;    
            
            var url = string.Format("{0}{1}", baseUrl, "token");
            Debug.WriteLine(REQ_ON_FORMAT, url);
            var x = typeof(TokenModel);
            var ser = new DataContractJsonSerializer(x);
            using (var stream = new MemoryStream(client.DownloadData(url)))
            {
                var tm = (TokenModel)ser.ReadObject(stream);
                return tm.access_token;
            }
        }


        public static ConnectorLocator Create(string userName, string password, string baseUrl)
        {
            return new ConnectorLocator(GetToken(userName, password, baseUrl), baseUrl);
        }
    }
}