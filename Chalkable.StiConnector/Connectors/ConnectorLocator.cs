using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Chalkable.Common.Exceptions;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorLocator
    {
        public const string REQ_ON_FORMAT = "Request on: {0}";
        public string BaseUrl { get; private set; }
        public string Token { get; private set; }
        public DateTime TokenExpires { get; private set; }
        public string ApiVersion { get; private set; }
        
        public ConnectorLocator(string token, string baseUrl, DateTime tokenExpires) 
            : this(token, baseUrl, tokenExpires, null)
        {
            ApiVersion = AboutConnector.GetApiVersion().Version;
        }

        public ConnectorLocator(string token, string baseUrl, DateTime tokenExpires, string apiVersion)
        {
            Token = token;
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";
            BaseUrl = baseUrl;
            TokenExpires = tokenExpires;
            InitServices();
            ApiVersion = apiVersion;
        }

        private void InitServices()
        {
            UsersConnector = new UsersConnector(this);
            AttendanceConnector = new AttendanceConnector(this);
            ActivityConnector = new ActivityConnector(this);
            AttachmentConnector = new AttachmentConnector(this);
            SectionStandardConnector = new SectionStandardConnector(this);
            ActivityScoreConnector = new ActivityScoreConnector(this);
            ReportConnector = new ReportConnector(this);
            SyncConnector = new SyncConnector(this);
            GradebookConnector = new GradebookConnector(this); 
            StandardScoreConnector = new StandardScoreConnector(this);
            SeatingChartConnector = new SeatingChartConnector(this);
            LinkConnector = new LinkConnector(this);
            DisciplineConnector = new DisciplineConnector(this);
            ActivityCategoryConnnector = new ActivityCategoryConnnector(this);
            StudentConnector = new StudentConnector(this);
            SectionCommentConnector = new SectionCommentConnector(this);
            ClassroomOptionConnector = new ClassroomOptionConnector(this);
            SectionDashboardConnector = new SectionDashboardConnector(this);
            LearningEarningsConnector = new LearningEarningsConnector(this);
            ActivityAssignedAttributeConnector = new ActivityAssignedAttributeConnector(this);
            GradingConnector = new GradingConnector(this);
            ClassesDashboardConnector = new ClassesDashboardConnector(this);
            AboutConnector = new AboutConnector(this);
        }

        public UsersConnector UsersConnector { get; private set; }
        public AttendanceConnector AttendanceConnector { get; private set; }
        public ActivityConnector ActivityConnector { get; private set; }
        public ActivityAssignedAttributeConnector ActivityAssignedAttributeConnector { get; private set; }
        public AttachmentConnector AttachmentConnector { get; private set; }
        public SectionStandardConnector SectionStandardConnector { get; private set; }
        public ActivityScoreConnector ActivityScoreConnector { get; private set; }
        public ReportConnector ReportConnector { get; private set; }
        public SyncConnector SyncConnector { get; private set; }
        public GradebookConnector GradebookConnector { get; private set; }
        public StandardScoreConnector StandardScoreConnector { get; private set; }
        public SeatingChartConnector SeatingChartConnector { get; private set; }
        public LinkConnector LinkConnector { get; private set; }
        public DisciplineConnector DisciplineConnector { get; private set; }
        public ActivityCategoryConnnector ActivityCategoryConnnector { get; private set; }
        public StudentConnector StudentConnector { get; private set; }
        public SectionCommentConnector SectionCommentConnector { get; private set; }
        public ClassroomOptionConnector ClassroomOptionConnector { get; private set; }
        public SectionDashboardConnector SectionDashboardConnector { get; private set; }
        public LearningEarningsConnector LearningEarningsConnector { get; private set; }
        public GradingConnector GradingConnector { get; private set; }
        public ClassesDashboardConnector ClassesDashboardConnector { get; private set; }
        public AboutConnector AboutConnector { get; set; }

        public class TokenModel
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }

        public static string GetToken(string userName, string password, string baseUrl, out DateTime expires)
        {
            expires = DateTime.Now;
            var client = new WebClient();
            string credentials = $"{userName}:{password}";
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string credentialsBase64 = Convert.ToBase64String(credentialsBytes);
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentialsBase64;
            client.Encoding = Encoding.UTF8;

            var url = $"{baseUrl}{"token"}";
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
                if (ex.Response != null)
                {
                    var reader = new StreamReader(ex.Response.GetResponseStream());

                    var msg = reader.ReadToEnd();
                    if (ex.Response is HttpWebResponse)
                    {
                        HttpStatusCode status = (ex.Response as HttpWebResponse).StatusCode;
                        if (status == HttpStatusCode.NotFound)
                            throw new ChalkableSisNotFoundException(ex.Message + Environment.NewLine + msg);
                        throw new HttpException((int)status, ex.Message + Environment.NewLine + msg);
                    }
                    throw new ChalkableException(ex.Message + Environment.NewLine + msg);    
                }
                throw new ChalkableException(ex.Message);

            }
            finally
            {
                stream?.Close();
            }
        }


        public static ConnectorLocator Create(string userName, string password, string baseUrl)
        {
            DateTime expires;
            var token = GetToken(userName, password, baseUrl, out expires);
            return new ConnectorLocator(token, baseUrl, expires);
        }
    }
}