using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Models;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public interface IDemoAnnouncementStorage
    {
        AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query);
        AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null);
        AnnouncementDetails GetDetails(int announcementId, int value, int id);
        Announcement GetById(int announcementId);
        void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state);
        void Update(Announcement ann);
        Announcement GetAnnouncement(int announcementId, int roleId, int value);
        Announcement GetLastDraft(int i);
        IList<Person> GetAnnouncementRecipientPersons(int announcementId, int userId);
        IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int i);
        bool Exists(string s, int classid, DateTime expiresDate, int? excludeAnnouncementId);
        void ReorderAnnouncements(int id, int value, int recipientId);
        bool CanAddStandard(int announcementId);
        bool IsEmpty();
        Dictionary<int, AnnouncementComplex> GetData();
        bool Exists(int id);
        void SetComplete(int announcementId, int userId, bool complete);
        void AddDemoAnnouncementsForClass(int classId);
        AnnouncementDetails SubmitAnnouncement(int? classId, AnnouncementDetails res);
        void DuplicateAnnouncement(int id, IList<int> classIds);
        void SetAnnouncementProcessor(IAnnouncementProcessor processor);

        IDemoAnnouncementStorage GetTeacherStorage();
    }


    public interface IAnnouncementProcessor
    {
        IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query);
        Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId);
    }

    class BaseAnnouncementProcessor : IAnnouncementProcessor
    {
        private DemoStorage Storage { get; set; }
        public BaseAnnouncementProcessor(DemoStorage storage)
        {
            Storage = storage;
        }

        public IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            return announcements;
        }

        public Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId)
        {
            throw new NotImplementedException();
        }
    }

    class TeacherAnnouncementProcessor : IAnnouncementProcessor
    {
        private DemoStorage Storage { get; set; }
        private IAnnouncementProcessor baseAnnouncementProcessor;

        public TeacherAnnouncementProcessor(DemoStorage storage)
        {
            Storage = storage;
            baseAnnouncementProcessor = new BaseAnnouncementProcessor(storage);
        }

        public IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            var res = baseAnnouncementProcessor.GetAnnouncements(announcements, query);
            return res.OrderByDescending(x => x.Id).ToList();
        }

        public Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId)
        {
            var announcementRecipients =
                Storage.AnnouncementRecipientStorage.GetAll()
                    .Where(x => x.ToAll || x.PersonRef == userId || x.RoleRef == roleId).Select(x => x.AnnouncementRef);

            return
                announcements.Where(x => x.Id == announcementId && x.PrimaryTeacherRef == userId || announcementRecipients.Contains(x.Id))
                    .Select(x => x)
                    .First();
        }
    }

    class StudentAnnouncementProcessor : IAnnouncementProcessor
    {
        private DemoStorage Storage { get; set; }
        private IAnnouncementProcessor baseAnnouncementProcessor;

        public StudentAnnouncementProcessor(DemoStorage storage)
        {
            Storage = storage;
            baseAnnouncementProcessor = new BaseAnnouncementProcessor(storage);
        }

        public IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            var res = baseAnnouncementProcessor.GetAnnouncements(announcements, query);
            return res.Where(x => x.VisibleForStudent).OrderByDescending(x => x.Id).ToList();
        }

        public Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId)
        {
            var classRefs = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
            {
                PersonId = userId
            }).Select(x => x.ClassRef).ToList();


            var gradeLevelRefs = Storage.StudentSchoolYearStorage.GetAll(userId).Select(x => x.GradeLevelRef).ToList();

            var annRecipients =
                Storage.AnnouncementRecipientStorage.GetAll()
                    .Where(
                        x =>
                            x.ToAll || x.PersonRef == userId || x.RoleRef == roleId ||
                            x.GradeLevelRef != null && gradeLevelRefs.Contains(x.GradeLevelRef.Value))
                    .Select(x => x.AnnouncementRef);

            return announcements.Where(x => x.Id == announcementId && classRefs.Contains(x.ClassRef) || annRecipients.Contains(x.Id))
                  .Select(x => x)
                  .First();
        }
    }


    

    class AdminAnnouncementProcessor : IAnnouncementProcessor
    {
        public IEnumerable<AnnouncementComplex> GetAnnouncements(IEnumerable<AnnouncementComplex> announcements, AnnouncementsQuery query)
        {
            return announcements;
        }

        public Announcement GetAnnouncement(IEnumerable<AnnouncementComplex> announcements, int announcementId, int roleId, int userId)
        {
            throw new NotImplementedException();
        }
    }


    public class DemoAnnouncementStorage : BaseDemoIntStorage<AnnouncementComplex>, IDemoAnnouncementStorage
    {
        private IAnnouncementProcessor announcementProcessor { get; set; }

        public DemoAnnouncementStorage(DemoStorage storage):base(storage, x => x.Id)
        {
            SetAnnouncementProcessor(new BaseAnnouncementProcessor(storage));
        }

        public DemoAnnouncementStorage(DemoStorage storage, Dictionary<int, AnnouncementComplex> anns, int lastIndex)
            : base(storage, x => x.Id)
        {
            data = anns;
            Index = lastIndex;
            SetAnnouncementProcessor(new BaseAnnouncementProcessor(storage));
        }

        public void SetAnnouncementProcessor(IAnnouncementProcessor processor)
        {
            announcementProcessor = processor;
        }

        public IDemoAnnouncementStorage GetTeacherStorage()
        {
            var storage = new DemoAnnouncementStorage(Storage, data, Index);
            storage.SetAnnouncementProcessor(new TeacherAnnouncementProcessor(Storage));
            return storage;
        }

        public bool CanAddStandard(int announcementId)
        {
            var announcement = GetById(announcementId);
            var cls = Storage.ClassStorage.GetById(announcement.ClassRef);  
            return Storage.ClasStandardStorage.GetAll().Count(x => x.ClassRef == cls.Id || x.ClassRef == cls.CourseRef) > 0;
        }

        public bool Exists(int id)
        {
            return data.Count(x => x.Value.SisActivityId == id) > 0;
        }

        public virtual AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            if (Storage.Context.PersonId == null)
                throw new ChalkableException("User local id is null");
            var announcements = data.Select(x => x.Value);
            if (query.Id.HasValue)
                announcements = announcements.Where(x => x.Id == query.Id);

            if (query.ClassId.HasValue)
                announcements = announcements.Where(x => x.ClassRef == query.ClassId);

            if (query.MarkingPeriodId.HasValue)
            {
                var mp = Storage.MarkingPeriodStorage.GetById(query.MarkingPeriodId.Value);
                announcements = announcements.Where(x => x.Expires >= mp.StartDate && x.Expires <= mp.EndDate);
            }

            if (query.FromDate.HasValue)
                announcements = announcements.Where(x => x.Expires >= query.FromDate);

            if (query.ToDate.HasValue)
                announcements = announcements.Where(x => x.Expires <= query.ToDate);
            if (query.Complete.HasValue)
                announcements = announcements.Where(x => Storage.AnnouncementCompleteStorage.GetComplete(x.Id, Storage.Context.PersonId.Value) == query.Complete);


            foreach (var announcementComplex in announcements)
            {
                var complete = Storage.AnnouncementCompleteStorage.GetComplete(announcementComplex.Id, Storage.Context.PersonId.Value);
                var cls = Storage.ClassStorage.GetById(announcementComplex.ClassRef);
                announcementComplex.FullClassName = cls.Name + " " + cls.ClassNumber;
                announcementComplex.Complete = complete.HasValue && complete.Value;
            }
            if (query.OwnedOnly)
                announcements = announcements.Where(x => x.PrimaryTeacherRef == query.PersonId);

            if (query.GradeLevelIds != null)
                announcements = announcements.Where(x => query.GradeLevelIds.Contains(x.Id));

            if (query.SisActivitiesIds != null)
                announcements = announcements.Where(x => query.SisActivitiesIds.Contains(x.Id));


            announcements = announcementProcessor.GetAnnouncements(announcements, query);

            if (query.Start > 0)
                announcements = announcements.Skip(query.Start);
            if (query.Count > 0)
                announcements = announcements.Take(query.Count);

            return new AnnouncementQueryResult
            {
                Announcements = announcements.ToList(),
                Query = query,
                SourceCount = data.Count
            };
        }


        private AnnouncementDetails ConvertToDetails(AnnouncementComplex announcement)
        {
            var announcementAttachments = Storage.AnnouncementAttachmentStorage.GetAll(announcement.Id);
            
            var announcementApplications= new List<AnnouncementApplication>();
            var announcementsQnA = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
            {
                AnnouncementId = announcement.Id
            }).AnnouncementQnAs;
            var announcementStandards =
                Storage.AnnouncementStandardStorage.GetAll(announcement.Id).Select(x => new AnnouncementStandardDetails
                {
                    AnnouncementRef = x.AnnouncementRef,
                    StandardRef = x.StandardRef,
                    Standard = Storage.StandardStorage.GetById(x.StandardRef)
                }).ToList();


            if (Storage.Context.PersonId == null) throw new Exception("Context user local id is null");

            var isComplete = Storage.AnnouncementCompleteStorage.GetComplete(announcement.Id,
                Storage.Context.PersonId.Value);

            var cls = Storage.ClassStorage.GetById(announcement.ClassRef);

            return new AnnouncementDetails
            {
                Id = announcement.Id,
                SisActivityId = announcement.SisActivityId,
                Complete = isComplete.HasValue && isComplete.Value,
                Subject = announcement.Subject,
                StudentsCount = announcement.StudentsCount,
                WeightAddition = announcement.WeightAddition,
                WeightMultiplier = announcement.WeightMultiplier,
                QnACount = announcement.QnACount,
                OwnerAttachmentsCount = announcement.OwnerAttachmentsCount,
                PrimaryTeacherRef = announcement.PrimaryTeacherRef,
                IsOwner = announcement.PrimaryTeacherRef == Storage.Context.PersonId,
                PrimaryTeacherName = announcement.PrimaryTeacherName,
                SchoolRef = announcement.SchoolRef,
                ClassRef = announcement.ClassRef,
                FullClassName = cls.Name + " " + cls.ClassNumber,
                PrimaryTeacherGender = announcement.PrimaryTeacherGender,
                ClassAnnouncementTypeRef = announcement.ClassAnnouncementTypeRef,
                Created = announcement.Created,
                Expires = announcement.Expires,
                GradingStyle = announcement.GradingStyle,
                State = announcement.State,
                IsScored = announcement.IsScored,
                Avg = announcement.Avg,
                Title = announcement.Title,
                Content = announcement.Content,
                AnnouncementAttachments = announcementAttachments,
                AnnouncementApplications = announcementApplications,
                AnnouncementQnAs = announcementsQnA,
                AnnouncementStandards = announcementStandards,
                Owner = announcement.PrimaryTeacherRef.HasValue ? Storage.PersonStorage.GetById(announcement.PrimaryTeacherRef.Value) : null,
                ApplicationCount = announcement.ApplicationCount,
                AttachmentsCount = announcement.AttachmentsCount,
                StudentAnnouncements = Storage.StudentAnnouncementStorage.GetAll(announcement.Id),
                VisibleForStudent = announcement.VisibleForStudent,
                MayBeDropped = announcement.MayBeDropped,
                Order = announcement.Order,
                ClassAnnouncementTypeName = announcement.ClassAnnouncementTypeName,
                ChalkableAnnouncementType = announcement.ChalkableAnnouncementType,
                ClassName = announcement.ClassName,
                GradeLevelId = announcement.GradeLevelId,
                MaxScore = announcement.MaxScore
            };
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null)
        {
            if (Storage.Context.SchoolLocalId == null) throw new Exception("Context school local id is null");

            var annId = GetNextFreeId();
            var person = Storage.PersonStorage.GetById(userId);
            var gradeLevelRef = Storage.ClassStorage.GetById(classId).GradeLevelRef;

            if (!classAnnouncementTypeId.HasValue)
                classAnnouncementTypeId = Storage.ClassAnnouncementTypeStorage.GetAll(classId).First().Id;

            //todo: create admin announcements if it's admin


            var cls = Storage.ClassStorage.GetById(classId);
            var announcement = new AnnouncementComplex
            {
                ClassAnnouncementTypeName = Storage.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId.Value).Name,
                ChalkableAnnouncementType = classAnnouncementTypeId,
                PrimaryTeacherName = person.FullName(),
                ClassName = cls.Name,
                GradeLevelId = gradeLevelRef,
                PrimaryTeacherGender = person.Gender,
                FullClassName = cls.Name + " " + cls.ClassNumber,
                IsScored = false,
                Id = annId,
                PrimaryTeacherRef = userId,
                IsOwner = Storage.Context.PersonId == userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = expiresDateTime.HasValue ? expiresDateTime.Value : DateTime.MinValue,
                State = AnnouncementState.Draft,
                GradingStyle = GradingStyleEnum.Numeric100,
                SchoolRef = Storage.Context.SchoolLocalId.Value,
                QnACount = 0,
                Order = annId,
                AttachmentsCount = 0,
                ApplicationCount = 0,
                OwnerAttachmentsCount = 0,
                StudentsCountWithAttachments = 0
            };
            

            data[announcement.Id] = announcement;
            return ConvertToDetails(announcement);
        }

        public AnnouncementDetails GetDetails(int announcementId, int userId, int roleId)
        {
            var announcement = GetAnnouncements(new AnnouncementsQuery
            {
                Id = announcementId,
                PersonId = userId,
                RoleId = roleId
            }).Announcements.First();

            return ConvertToDetails(announcement);
        }

        public new Announcement GetById(int announcementId)
        {
            return data.ContainsKey(announcementId) ? data[announcementId] : null;
        }

        public void AddDemoAnnouncementsForClass(int classId)
        {
            var types = Storage.ClassAnnouncementTypeStorage.GetAll(classId);

            foreach (var classAnnouncementType in types)
            {
                var classRef = classAnnouncementType.ClassRef;
                var announcementType = classAnnouncementType.Id;
                var announcementDetail = Create(announcementType, classRef, DateTime.Today, DemoSchoolConstants.TeacherId,
                    DateTime.Now.AddDays(1));
                var cls = Storage.ClassStorage.GetById(classRef);
                announcementDetail.ClassName = cls.Name;
                announcementDetail.FullClassName= cls.Name + " " + cls.ClassNumber;
                announcementDetail.IsScored = true;
                announcementDetail.MaxScore = 100;
                announcementDetail.Title = classAnnouncementType.Name + " " + classId;
                announcementDetail.VisibleForStudent = true;

                if (announcementDetail.AnnouncementStandards == null)
                    announcementDetail.AnnouncementStandards = new List<AnnouncementStandardDetails>();

                SubmitAnnouncement(classId, announcementDetail);
            }
        }

        public AnnouncementDetails SubmitAnnouncement(int? classId, AnnouncementDetails res)
        {
            var dateNow = Storage.Context.NowSchoolTime.Date;

            if (res.State == AnnouncementState.Draft)
            {
                res.State = AnnouncementState.Created;
                res.Created = dateNow;
                if (string.IsNullOrEmpty(res.Title) || res.DefaultTitle == res.Title)
                    res.Title = res.DefaultTitle;
                if (classId.HasValue)
                {
                    var activity = new Activity();
                    MapperFactory.GetMapper<Activity, AnnouncementDetails>().Map(activity, res);
                    activity = Storage.StiActivityStorage.CreateActivity(classId.Value, activity);
                    if (Exists(activity.Id))
                        throw new ChalkableException("Announcement with such activityId already exists");
                    res.SisActivityId = activity.Id;

                    var persons = Storage.PersonStorage.GetPersons(new PersonQuery
                    {
                        ClassId = classId,
                        RoleId = CoreRoles.STUDENT_ROLE.Id
                    }).Persons.Select(x => x.Id).ToList();


                    foreach (var personId in persons)
                    {
                        Storage.StudentAnnouncementStorage.Add(new StudentAnnouncement
                        {
                            AnnouncementId = res.Id,
                            ActivityId = activity.Id,
                            StudentId = personId
                        });

                        Storage.StiActivityScoreStorage.Add(new Score
                        {
                            ActivityId = activity.Id,
                            StudentId = personId
                        });
                    }


                    res.StudentsCount = persons.Count;


                }
            }
            res.GradingStyle = GradingStyleEnum.Numeric100;
            Update(res);
            return res;
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public void DuplicateAnnouncement(int id, IList<int> classIds)
        {
            var sourceAnnouncement = GetDetails(id, Storage.Context.PersonId.Value, Storage.Context.Role.Id);
            if (sourceAnnouncement.State != AnnouncementState.Created)
                throw new ChalkableException("Current announcement is not submited yet");
            if (!sourceAnnouncement.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement doesn't have activityId");

            foreach (var classId in classIds)
            {
                var announcement = Create(sourceAnnouncement.ClassAnnouncementTypeRef, classId, DateTime.Today,
                    Storage.Context.PersonId ?? 0);

                Storage.AnnouncementCompleteStorage.Add(new AnnouncementComplete
                {
                    AnnouncementId = announcement.Id,
                    Complete = sourceAnnouncement.Complete,
                    UserId = Storage.Context.PersonId.Value
                });
                announcement.Complete = sourceAnnouncement.Complete;
                announcement.Subject = sourceAnnouncement.Subject;
                announcement.WeightAddition = sourceAnnouncement.WeightAddition;
                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
                announcement.WeightMultiplier = sourceAnnouncement.WeightMultiplier;
                announcement.QnACount = sourceAnnouncement.QnACount;
                announcement.OwnerAttachmentsCount = sourceAnnouncement.OwnerAttachmentsCount;
                announcement.PrimaryTeacherRef = announcement.PrimaryTeacherRef;
                announcement.IsOwner = announcement.PrimaryTeacherRef == Storage.Context.PersonId;
                announcement.PrimaryTeacherName = sourceAnnouncement.PrimaryTeacherName;
                announcement.SchoolRef = sourceAnnouncement.SchoolRef;
                announcement.PrimaryTeacherGender = sourceAnnouncement.PrimaryTeacherGender;
                announcement.Expires = sourceAnnouncement.Expires;
                announcement.GradingStyle = sourceAnnouncement.GradingStyle;
                announcement.State = sourceAnnouncement.State;
                announcement.IsScored = sourceAnnouncement.IsScored;
                announcement.Avg = sourceAnnouncement.Avg;
                announcement.Title = sourceAnnouncement.Title;
                announcement.Content = sourceAnnouncement.Content;
                announcement.Owner = announcement.PrimaryTeacherRef.HasValue
                    ? Storage.PersonStorage.GetById(announcement.PrimaryTeacherRef.Value)
                    : null;
                announcement.ApplicationCount = sourceAnnouncement.ApplicationCount;
                announcement.AttachmentsCount = sourceAnnouncement.AttachmentsCount;
                announcement.VisibleForStudent = sourceAnnouncement.VisibleForStudent;
                announcement.MayBeDropped = sourceAnnouncement.MayBeDropped;
                announcement.Order = sourceAnnouncement.Order;
                announcement.ClassAnnouncementTypeName = sourceAnnouncement.ClassAnnouncementTypeName;
                announcement.ChalkableAnnouncementType = sourceAnnouncement.ChalkableAnnouncementType;
                announcement.ClassName = sourceAnnouncement.ClassName;
                announcement.GradeLevelId = sourceAnnouncement.GradeLevelId;
                announcement.MaxScore = sourceAnnouncement.MaxScore;
                // Added this copying 'cause of CHLK-3240
                announcement.SisActivityId = sourceAnnouncement.Id;

                var resAnnouncement = SubmitAnnouncement(classId, announcement);
                Storage.StiActivityScoreStorage.DuplicateFrom(sourceAnnouncement.SisActivityId.Value, resAnnouncement.SisActivityId.Value);
                var sourceStudentAnnouncements =
                        Storage.StudentAnnouncementStorage.GetAll()
                            .Where(x => x.AnnouncementId == sourceAnnouncement.Id)
                            .ToList();

                foreach (var sourceStudentAnnouncement in sourceStudentAnnouncements)
                {
                    Storage.StudentAnnouncementStorage.Add(new StudentAnnouncement
                    {
                        ActivityId = resAnnouncement.SisActivityId.Value,
                        StudentId = sourceStudentAnnouncement.StudentId,
                        Absent = sourceStudentAnnouncement.Absent,
                        AbsenceCategory = sourceStudentAnnouncement.AbsenceCategory,
                        AlphaGradeId = sourceStudentAnnouncement.AlphaGradeId,
                        AlternateScoreId = sourceStudentAnnouncement.AlternateScoreId,
                        Comment = sourceStudentAnnouncement.Comment,
                        Dropped = sourceStudentAnnouncement.Dropped,
                        AlternateScore = sourceStudentAnnouncement.AlternateScore,
                        Exempt = sourceStudentAnnouncement.Exempt,
                        Incomplete = sourceStudentAnnouncement.Incomplete,
                        Late = sourceStudentAnnouncement.Late,
                        NumericScore = sourceStudentAnnouncement.NumericScore,
                        OverMaxScore = sourceStudentAnnouncement.OverMaxScore,
                        ScoreValue = sourceStudentAnnouncement.ScoreValue,
                        Withdrawn = sourceStudentAnnouncement.Withdrawn,
                        AnnouncementId = resAnnouncement.Id,
                        AnnouncementTitle = sourceAnnouncement.Title,
                        ExtraCredit = sourceStudentAnnouncement.ExtraCredit,
                        IsWithdrawn = sourceStudentAnnouncement.IsWithdrawn
                    });
                }
                

                foreach (var sourceAnnouncementAttachment in sourceAnnouncement.AnnouncementAttachments)
                {
                    var newAnnAttachment = Storage.AnnouncementAttachmentStorage.Add(new AnnouncementAttachment()
                    {
                        AnnouncementRef = resAnnouncement.Id,
                        AttachedDate = sourceAnnouncementAttachment.AttachedDate,
                        Name = sourceAnnouncementAttachment.Name,
                        Order = sourceAnnouncementAttachment.Order,
                        PersonRef = sourceAnnouncementAttachment.PersonRef,
                        SisActivityId = resAnnouncement.SisActivityId
                    });


                    var content = Storage.BlobStorage.GetBlob(ATTACHMENT_CONTAINER_ADDRESS, sourceAnnouncementAttachment.Id.ToString(CultureInfo.InvariantCulture));
                    if (content != null)
                    {
                        Storage.BlobStorage.Add(ATTACHMENT_CONTAINER_ADDRESS, newAnnAttachment.Id.ToString(CultureInfo.InvariantCulture), content);
                    }
                }
                
                var announcementAttachments = Storage.AnnouncementAttachmentStorage.GetAll(resAnnouncement.Id);
                resAnnouncement.AnnouncementAttachments = announcementAttachments;
                var announcementApplications= new List<AnnouncementApplication>();

                resAnnouncement.StudentAnnouncements = Storage.StudentAnnouncementStorage.GetAll(resAnnouncement.Id);
                resAnnouncement.AnnouncementApplications = announcementApplications;

                foreach (var announcementQnAComplex in sourceAnnouncement.AnnouncementQnAs)
                {
                    Storage.AnnouncementQnAStorage.Add(new AnnouncementQnAComplex
                    {
                        AnnouncementRef = resAnnouncement.Id,
                        Answer = announcementQnAComplex.Answer,
                        AnswererRef = announcementQnAComplex.AnswererRef,
                        IsOwner = announcementQnAComplex.IsOwner,
                        Question = announcementQnAComplex.Question,
                        QuestionTime = announcementQnAComplex.QuestionTime,
                        State = announcementQnAComplex.State,
                        AskerRef = announcementQnAComplex.AskerRef,
                        Asker = announcementQnAComplex.Asker,
                        ClassRef = announcementQnAComplex.ClassRef,
                        Answerer = announcementQnAComplex.Answerer,
                        AnsweredTime = announcementQnAComplex.AnsweredTime
                    });
                }

                var announcementsQnA = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
                {
                    AnnouncementId = resAnnouncement.Id
                }).AnnouncementQnAs;
                resAnnouncement.AnnouncementQnAs = announcementsQnA;

                foreach (var announcementStandardDetails in sourceAnnouncement.AnnouncementStandards)
                {
                    Storage.AnnouncementStandardStorage.Add(new AnnouncementStandardDetails
                    {
                        AnnouncementRef = resAnnouncement.Id,
                        Standard = announcementStandardDetails.Standard,
                        StandardRef = announcementStandardDetails.StandardRef
                    });
                }

                var announcementStandards =
                    Storage.AnnouncementStandardStorage.GetAll(resAnnouncement.Id).Select(x => new AnnouncementStandardDetails
                    {
                        AnnouncementRef = x.AnnouncementRef,
                        StandardRef = x.StandardRef,
                        Standard = Storage.StandardStorage.GetById(x.StandardRef)
                    }).ToList();
                resAnnouncement.AnnouncementStandards = announcementStandards;

                Update(resAnnouncement);
            }
        }

        public void SetComplete(int announcementId, int userId, bool complete)
        {
            Storage.AnnouncementCompleteStorage.SetComplete
                (new AnnouncementComplete
                {
                    AnnouncementId = announcementId,
                    Complete = complete,
                    UserId = userId
                });
        }

        public void Delete(int? announcementId, int? userId, int? classId, int? announcementType, AnnouncementState? state)
        {
            var announcementsToDelete = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
                Id = announcementId,
                ClassId = classId
            }).Announcements;
            if (state.HasValue)
                announcementsToDelete = announcementsToDelete.Where(x => x.State == state).ToList();
            if (announcementType.HasValue)
                announcementsToDelete = announcementsToDelete.Where(x => x.ClassAnnouncementTypeRef == announcementType).ToList();

            foreach (var announcementComplex in announcementsToDelete)
            {
                if (announcementComplex.SisActivityId.HasValue)
                {
                    var scores = Storage.StiActivityScoreStorage.GetSores(announcementComplex.SisActivityId.Value);
                    Storage.StiActivityScoreStorage.Delete(scores);
                    var studentAnnouncements =
                        Storage.StudentAnnouncementStorage.GetAll()
                            .Where(x => x.AnnouncementId == announcementComplex.Id)
                            .ToList();
                    Storage.StudentAnnouncementStorage.Delete(studentAnnouncements);

                    var qnas = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
                    {
                        AnnouncementId = announcementComplex.Id
                    }).AnnouncementQnAs;

                    Storage.AnnouncementQnAStorage.Delete(qnas);
                    Storage.StiActivityStorage.Delete(announcementComplex.SisActivityId.Value);
                }
                var announcementApps = Storage.AnnouncementApplicationStorage.GetAll(announcementComplex.Id, true);
                Storage.AnnouncementApplicationStorage.Delete(announcementApps);

                var attachments = Storage.AnnouncementAttachmentStorage.GetAll(announcementComplex.Id);
                Storage.AnnouncementAttachmentStorage.Delete(attachments);
                var standarts = Storage.AnnouncementStandardStorage.GetAll(announcementComplex.Id);
                Storage.AnnouncementStandardStorage.Delete(standarts);
            }
            
            Delete(announcementsToDelete.Select(x => x.Id).ToList());
        }

        public void Update(Announcement ann)
        {
            if (data.ContainsKey(ann.Id))
            {
                data[ann.Id].Content = ann.Content;
                data[ann.Id].Created = ann.Created;
                data[ann.Id].Expires = ann.Expires;
                data[ann.Id].ClassAnnouncementTypeRef = ann.ClassAnnouncementTypeRef;
                data[ann.Id].State = ann.State;
                data[ann.Id].GradingStyle = ann.GradingStyle;
                data[ann.Id].Subject = ann.Subject;
                data[ann.Id].ClassRef = ann.ClassRef;
                data[ann.Id].Order = ann.Order;
                data[ann.Id].Dropped = ann.Dropped;
                data[ann.Id].MayBeDropped = ann.MayBeDropped;
                data[ann.Id].VisibleForStudent = ann.VisibleForStudent;
                data[ann.Id].SchoolRef = ann.SchoolRef;
                data[ann.Id].Title = ann.Title;
                data[ann.Id].SisActivityId = ann.SisActivityId;
                data[ann.Id].MaxScore = ann.MaxScore;
                data[ann.Id].WeightAddition = ann.WeightAddition;
                data[ann.Id].WeightMultiplier = ann.WeightMultiplier;
                data[ann.Id].MayBeDropped = ann.MayBeDropped;
                data[ann.Id].MayBeExempt = ann.MayBeExempt;
                data[ann.Id].IsScored = ann.IsScored;
            }
        }

        public Announcement GetAnnouncement(int announcementId, int roleId, int userId)
        {
            return announcementProcessor.GetAnnouncement(data.Select(x => x.Value), announcementId, roleId, userId);
        }

        public Announcement GetLastDraft(int userId)
        {
            return GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = userId,
            }).Announcements.Where(x => x.State == AnnouncementState.Draft).OrderByDescending(x => x.Id).FirstOrDefault();
        }

        public IList<Person> GetAnnouncementRecipientPersons(int announcementId, int userId)
        {
            var annRecipients = Storage.AnnouncementRecipientStorage.GetList(announcementId);
            var result = new List<Person>();
            foreach (var announcementRecipient in annRecipients)
            {
                if (announcementRecipient.PersonRef.HasValue)
                    result.Add(Storage.PersonStorage.GetPerson(new PersonQuery
                    {
                        PersonId = announcementRecipient.PersonRef
                    }));
            }
            return result;
        }

        public IList<string> GetLastFieldValues(int personId, int classId, int classAnnouncementType, int count)
        {
            var announcements = GetAnnouncements(new AnnouncementsQuery
            {
                PersonId = personId,
                ClassId = classId,
            }).Announcements.ToList();
            
            return announcements.Where(x => x.ClassAnnouncementTypeRef == classAnnouncementType).Take(count).Select(x => x.Content).ToList();

        }

        public bool Exists(string s, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return data.Count(x => x.Value.Title == s && x.Value.ClassRef == classId && x.Value.Expires.Date == expiresDate.Date && excludeAnnouncementId != x.Value.Id ) > 0;
        }

        public void ReorderAnnouncements(int schoolYearId, int classAnnouncementTypeId, int recipientClassId)
        {
        }
    }
}
