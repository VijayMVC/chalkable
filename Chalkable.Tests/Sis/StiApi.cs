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
using PersonLanguage = Chalkable.StiConnector.SyncModel.PersonLanguage;


namespace Chalkable.Tests.Sis
{
    public partial class StiApi : TestBase
    {
        private SyncResult<T> GetTableData<T>(Guid districtId, long? version) where T : SyncModel
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";

            District d;
            IList<Data.Master.Model.User> existingUsers;
            using (var uow = new UnitOfWork(mcs, false))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtId);
                var conds = new SimpleQueryCondition("DistrictRef", districtId, ConditionRelation.Equal);
                existingUsers = (new UserDataAccess(uow)).GetAll(conds);
            }
            var cl = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
            var items = (cl.SyncConnector.GetDiff(typeof(T), null) as SyncResult<T>);
            return items;
        }

        [Test]
        public void DisableStudentSchoolYearHomeroomConstraint()
        {
            var districtIds = new List<Guid>
            {
                Guid.Parse("341D63DB-49AE-4057-A06D-714E9A3D31EB"),
                Guid.Parse("aa707676-1548-4cdc-af84-2a15257c95bd"),
                //Guid.Parse("53d7cbd7-cd3e-4a50-9258-8c1f6daedb61"),
                //Guid.Parse("6c801aac-4d1c-4a70-8e06-d82ef6846a31"),
                //Guid.Parse("c9c9ad32-1576-45e6-8f92-39de929f4554"),
                //Guid.Parse("26a3a198-e329-4a34-a6e0-814c76ad45cb"),
                //Guid.Parse("747BC5D3-C453-42F3-AF54-868351ACFEE4"),
                //Guid.Parse("27e81a8d-772c-430f-8886-24b3b3d21df8"),
                //Guid.Parse("509138b6-db56-45ee-8c26-592abe4e1c5a"),
                //Guid.Parse("77a5f7c0-a97e-448f-b619-72799719dc78"),
                //Guid.Parse("ef4f14b7-13d0-4710-95c3-124537284880"),
                //Guid.Parse("0972d700-06c4-49f9-9b69-ad76e07a76f8"),
            };
            ForEachDistrict(districtIds, delegate(District d, UnitOfWork u)
            {
                var c = u.GetTextCommand("ALTER TABLE StudentSchoolYear NOCHECK CONSTRAINT FK_StudentSchoolYear_Homeroom");
                //ALTER TABLE StudentSchoolYear WITH CHECK CHECK CONSTRAINT FK_StudentSchoolYear_Homeroom
                c.ExecuteNonQuery();
            });
            //
        }

        [Test]
        public void FixGradeLevelNumber()
        {
            var districtIds = new List<Guid>
            {
                Guid.Parse("18c60c7d-c931-4e6f-90ac-2dc85e958efd"),

                Guid.Parse("6afa2170-6a18-43d2-b4a0-81a42c6a75dd"),

                Guid.Parse("1ebcbf07-bcad-4c75-b692-6298a164f9b9"),

                //Guid.Parse("667416fc-7ece-40bb-b931-1273c495280d"),

                //Guid.Parse("9067d336-bfbd-410d-856c-ba5ea5ae91fa"),

                
            };
            ForEachDistrict(districtIds, delegate (District d, UnitOfWork u)
            {
                var c = u.GetTextCommand("Update GradeLevel set Number = Number + 100");
                c.ExecuteNonQuery();
            });
            //
        }

        [Test]
        public void SyncTest()
        {
            var items = GetTableData<PersonLanguage>(Guid.Parse("CDB64B27-54E4-40B4-8807-C4037867E751"), null);
            Print(items.All);
            //Print(items.Updated);
            //Print(items.Deleted);
        }
        
        [Test]
        public void FixBellScheduleDelete()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("27e81a8d-772c-430f-8886-24b3b3d21df8"),
                //Guid.Parse("1ebcbf07-bcad-4c75-b692-6298a164f9b9"),
                //Guid.Parse("42bf4a08-5a1a-488a-9a41-812421e21df8"),
                //Guid.Parse("76ec8972-ca7a-4faf-adb2-f9c48adfeed6"),
                

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
                Guid.Parse("e02c0198-b69b-47f6-871e-c4de3ecbbe1e"),
                Guid.Parse("f754b2e4-d360-4922-8c9a-d5e21fc3007c"),
                Guid.Parse("cdb64b27-54e4-40b4-8807-c4037867e751"),
                //Guid.Parse("f754b2e4-d360-4922-8c9a-d5e21fc3007c"),
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
                Guid.Parse("66B7AA33-2F40-4568-A222-4A99AB62955B"),
                Guid.Parse("FC507B44-64C3-40B6-8082-9FDC4B0EA33A"),
                Guid.Parse("6C801AAC-4D1C-4A70-8E06-D82EF6846A31"),

                //Guid.Parse("12628575-A1F2-4DB7-B421-D2DE7DAC343F"),
                //Guid.Parse("0B55D8F2-6FD7-45FE-8944-EE4EDC86DA79"),
                //Guid.Parse("CDB64B27-54E4-40B4-8807-C4037867E751"),


            };
            foreach (var guid in ids)
            {
                try
                {
                    FixUserSchoolSync(guid);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
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
