using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.SectionPanorama;
using Chalkable.StiConnector.SyncModel;
using Newtonsoft.Json;
using NUnit.Framework;
using Address = Chalkable.StiConnector.SyncModel.Address;
using AttendanceMonth = Chalkable.StiConnector.SyncModel.AttendanceMonth;
using ContactRelationship = Chalkable.StiConnector.SyncModel.ContactRelationship;
using CourseType = Chalkable.StiConnector.SyncModel.CourseType;
using District = Chalkable.Data.Master.Model.District;
using GradedItem = Chalkable.StiConnector.SyncModel.GradedItem;
using GradeLevel = Chalkable.StiConnector.SyncModel.GradeLevel;
using GradingScale = Chalkable.StiConnector.SyncModel.GradingScale;
using Person = Chalkable.StiConnector.SyncModel.Person;
using Room = Chalkable.StiConnector.SyncModel.Room;
using Standard = Chalkable.StiConnector.SyncModel.Standard;
using Student = Chalkable.StiConnector.SyncModel.Student;
using StudentContact = Chalkable.StiConnector.SyncModel.StudentContact;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;
using User = Chalkable.StiConnector.SyncModel.User;
using UserSchool = Chalkable.StiConnector.SyncModel.UserSchool;
using BellSchedule = Chalkable.StiConnector.SyncModel.BellSchedule;
using Infraction = Chalkable.StiConnector.SyncModel.Infraction;


namespace Chalkable.Tests.Sis
{
    public partial class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {
            var cl = ConnectorLocator.Create("Chalkable", "g5Hk4By4V", "https://inowhome.dcs.edu/Api/");
            var items = (cl.SyncConnector.GetDiff(typeof(Infraction), null) as SyncResult<Infraction>);
            Print(items.All.Where(x=>x.VisibleInClassroom == true));
            //Print(items.Updated);
            //Print(items.Deleted);
        }
        
        [Test]
        public void FixBellScheduleDelete()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("628F4CA2-7232-418F-A42E-F859286E912A"),
                Guid.Parse("9209de50-0bbe-40f4-8e50-cf52ce39cb2b"),
                
            };
            foreach (var guid in ids)
            {
                FixBellScheduleDelete(guid);
            }
        }

        [Test]
        public void FixRoomDelete()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("ccf4b45d-3357-43ae-9ee8-66949e64279e"),

            };
            foreach (var guid in ids)
            {
                FixRoomDelete(guid);
            }
        }



        [Test]
        public void FixUserSchoolSync()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("F14ABF1C-102B-4B4E-ACC3-1367F6F31069"),

                //Guid.Parse("428CA1B1-BC79-4096-981A-955EF5B2A74B"),

                //Guid.Parse("77A5F7C0-A97E-448F-B619-72799719DC78"), //!!!!!!!!!!!! constantly repeats

                //Guid.Parse("E02C0198-B69B-47F6-871E-C4DE3ECBBE1E")
            };
            foreach (var guid in ids)
            {
                FixUserSchoolSync(guid);
            }
        }

        

        [Test]
        public void FixUserSyncDistricts()
        {
            FixUserSyncAllDistricts();
        }

        public void FixUserSyncAllDistricts()
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            IList<District> districts;
            using (var uow = new UnitOfWork(mcs, true))
            {
                var da = new DistrictDataAccess(uow);
                districts = da.GetAll();
            }
            int cnt = 30;
            List<District>[] lists = new List<District>[cnt];
            for (int i = 0; i < cnt; i++)
                lists[i] = new List<District>();
            for (int i = 0; i < districts.Count; i++)
            {
                lists[i%30].Add(districts[i]);
            }
            Thread[] threads = new Thread[cnt];
            for (int i = 0; i < cnt; i++)
            {
                int ii = i;
                var t = new Thread(() =>
                {
                    int k = ii;
                    for (int j = 0; j < lists[k].Count; j++)
                    {
                        FixUserSync(lists[k][j].Id);
                        Debug.WriteLine($"{k} {j} completed");
                    }
                });
                threads[i] = t;
                t.Start();
            }
            for (int i = 0; i < cnt; i++)
                threads[i].Join();
        }

        public void FixUserSync(Guid districtid)
        {
            StringBuilder log = new StringBuilder();
            try
            {
                var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

                District d;
                IList<Data.Master.Model.User> chalkableUsers;
                using (var uow = new UnitOfWork(mcs, true))
                {
                    var da = new DistrictDataAccess(uow);
                    d = da.GetById(districtid);
                    var conds = new SimpleQueryCondition("DistrictRef", districtid, ConditionRelation.Equal);
                    chalkableUsers = (new UserDataAccess(uow)).GetAll(conds);
                }
                //var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);

                var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
                var inowUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
                var st = new HashSet<int>(chalkableUsers.Select(x => x.SisUserId.Value).ToList());

                IList<Data.Master.Model.User> users = new List<Data.Master.Model.User>();
                foreach (var sisu in inowUsers)
                    if (!st.Contains(sisu.UserID))
                    {
                        Data.Master.Model.User u = new Data.Master.Model.User
                        {
                            Id = Guid.NewGuid(),
                            DistrictRef = districtid,
                            FullName = sisu.FullName,
                            Login = String.Format("user{0}_{1}@chalkable.com", sisu.UserID, districtid),
                            Password = "1Ztq1N1GZ95sasjFa54ikw==",
                            SisUserName = sisu.UserName,
                            SisUserId = sisu.UserID
                        };
                        users.Add(u);
                        log.AppendLine(sisu.UserID.ToString());
                    }

                using (var uow = new UnitOfWork(mcs, true))
                {

                    (new UserDataAccess(uow)).Insert(users);
                    uow.Commit();
                }
                log.AppendLine($"{users.Count} users were added");
            }
            catch (Exception ex)
            {
                log.AppendLine(ex.Message);
                log.AppendLine(ex.StackTrace);
            }
            
            File.WriteAllText($"c:\\tmp\\logs\\{districtid}.txt", log.ToString());
        }

        

        [Test]
        public void Test4()
        {
            var cl = ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/chalkable/api/");
            var items = (cl.SyncConnector.GetDiff(typeof(AcadSession), null) as SyncResult<AcadSession>).All.ToList();
            items = items.ToList();

            var sql = new StringBuilder();
            sql.Append(@"declare @sy table (id int, ArchiveDate datetime2 null) 
                         insert into @sy
                         values");

            foreach ( var item in items)
            {
                var s = string.Format("({0},{1}),", item.AcadSessionID, item.ArchiveDate.HasValue ? "cast('" + item.ArchiveDate.Value + "' as datetime2)" : "null");
                sql.Append(s);
            }
            sql.Append(" ").Append(@"update SchoolYear
                                    set ArchiveDate = sy.ArchiveDate
                                    from SchoolYear 
                                    join @sy sy on SchoolYear.Id = sy.Id");

        
            Debug.WriteLine(sql.ToString());
        }

        [Test]
        public void Test3()
        {
            Debug.WriteLine(DateTime.Now.Month);
        }

        [Test]
        public void SectionPanoramaApiTest()
        {
            var connector = ConnectorLocator.Create("MAGOLDEN-3856695863", "qqqq1111", "http://sandbox.sti-k12.com/chalkable/api/");
            var componentsIds = new List<int>
            {1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 6, 7, 7, 7, 7, 7, 7, 8, 10, 11, 12, 13, 14, 15};

            var scoreTypeIds = new List<int>
            {1, 2, 3, 4, 5, 6, 1, 2, 3, 4, 5, 6, 11, 1, 2, 3, 4, 5, 6, 8, 12, 12, 12, 12, 12, 12};

            Debug.WriteLine($"{componentsIds.Count} - {scoreTypeIds.Count}");

            SectionPanorama callResult = new SectionPanorama();
            List<int> classIds = new List<int>
            {13770, 13771, 13772, 13806, 13861, 13862, 13950, 14011, 14436, 15165};

            bool found = false;
            int @class = 0;
            foreach (var classId in classIds)
            {
                callResult = connector.PanoramaConnector.GetSectionPanorama(classId, new List<int> {179}, componentsIds, scoreTypeIds);
                if (callResult.StandardizedTests != null)
                {
                    found = true;
                    @class = classId;
                    break;
                }
            }
            if (!found)
            {
                Debug.WriteLine("StandardizedTests not found in any class");
                return;
            }

            Debug.WriteLine($"ClassId : {@class}");
            Debug.WriteLine("Absences [StudentId - NumberOfAbsences - NumberOfDaysEnrolled]:");
            foreach (var studentAbsence in callResult.Absences)
            {
                Debug.WriteLine($"{studentAbsence.StudentId} - {studentAbsence.NumberOfAbsences} - {studentAbsence.NumberOfDaysEnrolled}");
            }

            Debug.WriteLine("Grades [StudentId - AvarageGrade]:");
            foreach (var grade in callResult.Grades)
            {
                Debug.WriteLine($"{grade.StudentId} - {grade.AverageGrade}");
            }
            
            Debug.WriteLine("Infractions [StudentId - NumberOfInfractions]:");
            foreach (var inf in callResult.Infractions)
            {
                Debug.WriteLine($"{inf.StudentId} - {inf.NumberOfInfractions}");
            }

            Debug.WriteLine("StandardizedTests [StudentId - Date - Score - StandardizedTestComponentId - StandardizedTestScoreTypeId]:");
            if(callResult.StandardizedTests != null)
                foreach (var stTests in callResult.StandardizedTests)
                {
                    Debug.WriteLine($"{stTests.StudentId} - {stTests.Date} - {stTests.Score} - {stTests.StandardizedTestComponentId} - {stTests.StandardizedTestId} - {stTests.StandardizedTestScoreTypeId}");
                }
        }

        [Test]
        public void StudentPanoramaApiTest()
        {
            var studentId = 3315;
            var connector = ConnectorLocator.Create("MAGOLDEN-3856695863", "qqqq1111", "http://sandbox.sti-k12.com/chalkable/api/");
            var componentsIds = new List<int>
            {1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 6, 7, 7, 7, 7, 7, 7, 8, 10, 11, 12, 13, 14, 15};
            var scoreTypeIds = new List<int>
            {1, 2, 3, 4, 5, 6, 1, 2, 3, 4, 5, 6, 11, 1, 2, 3, 4, 5, 6, 8, 12, 12, 12, 12, 12, 12};
            var acadSessionIds = new List<int> {179};

            var studentPanorama = connector.PanoramaConnector.GetStudentPanorama(studentId, acadSessionIds, componentsIds, scoreTypeIds);
            
            if (studentPanorama.StandardizedTests != null)
            {
                Debug.WriteLine("Standardized Tests:");
                Debug.WriteLine(JsonConvert.SerializeObject(studentPanorama.StandardizedTests));
            }

            if (studentPanorama.Infractions != null)
            {
                Debug.WriteLine("Infractions:");
                Debug.WriteLine(JsonConvert.SerializeObject(studentPanorama.Infractions));
            }

            if (studentPanorama.DailyAbsences != null)
            {
                Debug.WriteLine("DailyAbsences:");
                Debug.WriteLine(JsonConvert.SerializeObject(studentPanorama.DailyAbsences));
            }

            if (studentPanorama.PeriodAbsences != null)
            {
                Debug.WriteLine("PeriodAbsences:");
                Debug.WriteLine(JsonConvert.SerializeObject(studentPanorama.PeriodAbsences));
            }
        }

        class WebClientGZip : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                //request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
#if DEBUG
                request.Timeout = 30000;
#else
                request.Timeout = Settings.WebClientTimeout;
#endif
                return request;
            }
        }

        [Test]
        public void GetSyncTablesProps()
        {
            var connector =  ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/chalkable/api/");
            var client = new WebClientGZip
            {
                Headers =
                {
                    [HttpRequestHeader.Authorization] = "Session " + connector.Token,
                    ["ApplicationKey"] = $"chalkable {Settings.StiApplicationKey}",
                    ["Accept-Encoding"] = "gzip, deflate"
                },
                Encoding = Encoding.UTF8
            };
            client.Headers.Add("Content-Type", "application/json");
            try
            {
                var url = connector.BaseUrl + $"sync/tables/Homeroom/";

                client.QueryString = new NameValueCollection();
                var data = client.DownloadData(url);

                using (var ms = new MemoryStream(data))
                {
                    StreamReader reader;
                    GZipStream unzipped = null;
                    if (client.ResponseHeaders[HttpResponseHeader.ContentType].ToLower() == "application/octet-stream")
                    {
                        unzipped = new GZipStream(ms, CompressionMode.Decompress);
                        reader = new StreamReader(unzipped);
                    }
                    else
                        reader = new StreamReader(ms);
                    
                    Debug.WriteLine(reader.ReadToEnd());

                    unzipped?.Dispose();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse &&
                    (ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                    return;

                string msg = ex.Message;
                var stream = ex.Response?.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    msg = reader.ReadToEnd();
                }
                throw new Exception(msg);
            }
        }
    }
}
