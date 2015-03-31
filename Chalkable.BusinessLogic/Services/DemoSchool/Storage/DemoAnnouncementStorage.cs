using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.announcement;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;

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
        IList<AnnouncementComplex> GetByActivitiesIds(IList<int> importantActivitiesIds);
    }
    
    public class DemoAnnouncementStorage : BaseDemoIntStorage<AnnouncementComplex>, IDemoAnnouncementStorage
    {
       
        public DemoAnnouncementStorage():base(x => x.Id)
        {
               
        }

        private void SetupAnnouncementProcessor(UserContext context, DemoStorageLocator locator)
        {
            if (BaseSecurity.IsAdminViewer(context))
                SetAnnouncementProcessor(new AdminAnnouncementProcessor(locator));
            if (Context.Role == CoreRoles.TEACHER_ROLE)
                SetAnnouncementProcessor(new TeacherAnnouncementProcessor(locator));
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                SetAnnouncementProcessor(new StudentAnnouncementProcessor(StorageLocator));
        }


        public override void Setup(DemoStorageLocator storageLocator, UserContext context)
        {
            base.Setup(storageLocator, context);
            SetupAnnouncementProcessor(context, storageLocator);
        }

        public DemoAnnouncementStorage(Dictionary<int, AnnouncementComplex> anns, int lastIndex)
            : base(x => x.Id)
        {
            data = anns;
            Index = lastIndex;
        }

        public void SetAnnouncementProcessor(IAnnouncementProcessor processor)
        {
            announcementProcessor = processor;
        }

        public IDemoAnnouncementStorage GetTeacherStorage()
        {
            var storage = new DemoAnnouncementStorage(data, Index);
            storage.Setup(StorageLocator, Context);
            return storage;
        }

       
        public bool Exists(int id)
        {
            return data.Count(x => x.Value.SisActivityId == id) > 0;
        }

        public new Announcement GetById(int announcementId)
        {
            return data.ContainsKey(announcementId) ? data[announcementId] : null;
        }

        public AnnouncementDetails Create(int? classAnnouncementTypeId, int classId, DateTime nowLocalDate, int userId, DateTime? expiresDateTime = null)
        {
            if (Context.SchoolLocalId == null) throw new Exception("Context school local id is null");

            var annId = GetNextFreeId();
            var person = StorageLocator.PersonStorage.GetById(userId);

            if (!classAnnouncementTypeId.HasValue)
                classAnnouncementTypeId = StorageLocator.ClassAnnouncementTypeStorage.GetAll(classId).First().Id;

            //todo: create admin announcements if it's admin


            var cls = StorageLocator.ClassStorage.GetById(classId);
            var announcement = new AnnouncementComplex
            {
                ClassAnnouncementTypeName = StorageLocator.ClassAnnouncementTypeStorage.GetById(classAnnouncementTypeId.Value).Name,
                ChalkableAnnouncementType = classAnnouncementTypeId,
                PrimaryTeacherName = person.FullName(),
                ClassName = cls.Name,
                PrimaryTeacherGender = person.Gender,
                FullClassName = cls.Name + " " + cls.ClassNumber,
                IsScored = false,
                Id = annId,
                PrimaryTeacherRef = userId,
                IsOwner = Context.PersonId == userId,
                ClassRef = classId,
                ClassAnnouncementTypeRef = classAnnouncementTypeId,
                Created = nowLocalDate,
                Expires = expiresDateTime.HasValue ? expiresDateTime.Value : DateTime.MinValue,
                State = AnnouncementState.Draft,
                GradingStyle = GradingStyleEnum.Numeric100,
                SchoolRef = Context.SchoolLocalId.Value,
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

       

        

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        

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
                    var scores = StorageLocator.StiActivityScoreStorage.GetSores(announcementComplex.SisActivityId.Value);
                    StorageLocator.StiActivityScoreStorage.Delete(scores);
                    var studentAnnouncements =
                        StorageLocator.StudentAnnouncementStorage.GetAll()
                            .Where(x => x.AnnouncementId == announcementComplex.Id)
                            .ToList();
                    StorageLocator.StudentAnnouncementStorage.Delete(studentAnnouncements);

                    var qnas = StorageLocator.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
                    {
                        AnnouncementId = announcementComplex.Id
                    }).AnnouncementQnAs;

                    StorageLocator.AnnouncementQnAStorage.Delete(qnas);
                    StorageLocator.StiActivityStorage.Delete(announcementComplex.SisActivityId.Value);
                }
                var announcementApps = StorageLocator.AnnouncementApplicationStorage.GetAll(announcementComplex.Id, true);
                StorageLocator.AnnouncementApplicationStorage.Delete(announcementApps);

                var attachments = StorageLocator.AnnouncementAttachmentStorage.GetAll(announcementComplex.Id);
                StorageLocator.AnnouncementAttachmentStorage.Delete(attachments);
                var standarts = StorageLocator.AnnouncementStandardStorage.GetAll(announcementComplex.Id);
                StorageLocator.AnnouncementStandardStorage.Delete(standarts);
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

        public bool Exists(string s, int classId, DateTime expiresDate, int? excludeAnnouncementId)
        {
            return data.Count(x => x.Value.Title == s && x.Value.ClassRef == classId && x.Value.Expires.Date == expiresDate.Date && excludeAnnouncementId != x.Value.Id ) > 0;
        }

    }
}
