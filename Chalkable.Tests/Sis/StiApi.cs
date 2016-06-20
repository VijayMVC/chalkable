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
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
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


namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void SyncTest()
        {
            var cl = ConnectorLocator.Create("Chalkable", "Gq1Yo2Rp6", "https://519217.stiinformationnow.com/API/");
            var items = (cl.SyncConnector.GetDiff(typeof(CourseType), 1706917) as SyncResult<CourseType>);
            Print(items.Inserted);
            Print(items.Updated);
            Print(items.Deleted);
        }

        [Test]
        public void FixUserSchoolSync()
        {
            var ids = new List<Guid>
            {
                Guid.Parse("CDB64B27-54E4-40B4-8807-C4037867E751"),

                Guid.Parse("F76407F1-5AD1-4B92-BE5F-659DC3E15BF1"),

                Guid.Parse("FC507B44-64C3-40B6-8082-9FDC4B0EA33A"),

                Guid.Parse("E02C0198-B69B-47F6-871E-C4DE3ECBBE1E"),

                Guid.Parse("1C46F721-D79F-40C4-A0B6-68D3D0A73D82"),
            };
            foreach (var guid in ids)
            {
                FixUserSchoolSync(guid);
            }
        }

        public void FixUserSchoolSync(Guid districtid)
        {
            var mcs = "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!";
            
            District d;
            IList<Data.Master.Model.User> existingUsers;
            using (var uow = new UnitOfWork(mcs, true))
            {
                var da = new DistrictDataAccess(uow);
                d = da.GetById(districtid);
                var conds = new SimpleQueryCondition("DistrictRef", districtid, ConditionRelation.Equal);
                existingUsers = (new UserDataAccess(uow)).GetAll(conds);

                uow.Commit();
            }
            var cs = String.Format("Data Source={0};Initial Catalog={1};UID=chalkableadmin;Pwd=Hellowebapps1!", d.ServerUrl, d.Id);
            IList<SyncVersion> versions;
            using (var uow = new UnitOfWork(cs, true))
            {
                versions = (new SyncVersionDataAccess(uow)).GetAll();
                uow.Commit();
            }

            var cl = ConnectorLocator.Create("Chalkable", d.SisPassword, d.SisUrl);
            var addedUsers = (cl.SyncConnector.GetDiff(typeof(User), versions.First(x=>x.TableName=="User").Version) as SyncResult<User>).Inserted;
            var AllUsers = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
            var addedUserSchools = (cl.SyncConnector.GetDiff(typeof(UserSchool), versions.First(x => x.TableName == "UserSchool").Version) as SyncResult<UserSchool>).Inserted;

            IList<Data.Master.Model.User> users = new List<Data.Master.Model.User>();
            var ids = addedUserSchools.Select(x => x.UserID).Distinct();
            foreach (var addedUserSchool in ids)
            {
                if (existingUsers.All(x => x.SisUserId != addedUserSchool))
                {
                    if (addedUsers.All(x => x.UserID != addedUserSchool))
                    {
                        var sisu = AllUsers.First(x => x.UserID == addedUserSchool);
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
                    }
                }
            }

            
            using (var uow = new UnitOfWork(mcs, true))
            {
                
                (new UserDataAccess(uow)).Insert(users);
                uow.Commit();
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

        private void Print(IEnumerable<Standard> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StandardID} {item.ParentStandardID} {item.StandardSubjectID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Person> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.PersonID}, {item.FirstName}, {item.LastName}, {item.DateOfBirth}, {item.GenderDescriptor}, {item.PhysicalAddressID}, {item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<StudentScheduleTerm> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.SectionID} {item.TermID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<PersonTelephone> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.PersonID} {item.TelephoneNumber} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }
        
        private void Print(IEnumerable<Course> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.CourseID} {item.CourseTypeID} {item.GradingScaleID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<User> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }
        
        private void Print(IEnumerable<UserSchool> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.UserID} {item.SchoolID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Room> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.RoomID} {item.RoomNumber} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<GradeLevel> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.GradeLevelID} {item.Name} {item.Sequence} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<StudentContact> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.RelationshipID}  {item.ContactID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<ContactRelationship> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.ContactRelationshipID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<GradingScale> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.GradingScaleID} {item.Name} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Student> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.StudentID} {item.UserID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<Address> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.AddressID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
        }

        private void Print(IEnumerable<CourseType> items)
        {
            foreach (var item in items)
            {
                Debug.WriteLine($"{item.CourseTypeID} {item.SYS_CHANGE_VERSION} {item.SYS_CHANGE_CREATION_VERSION}");
            }
            Debug.WriteLine("---------------------------------");
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
        public void PanoramaApiTest()
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
                var url = connector.BaseUrl + $"sync/tables/PersonNationality/";

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

                    var serializer = new JsonSerializer();
                    var jsonReader = new JsonTextReader(reader);
                    
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
