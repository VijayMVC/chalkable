using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoStudentAnnouncementService : DemoSisConnectedService, IStudentAnnouncementService
    {
        public DemoStudentAnnouncementService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }


        //TODO : needs testing 
        public StudentAnnouncement SetGrade(int studentAnnouncementId, string value, string extraCredits, string comment, bool dropped,
                                            bool late, bool absent, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            var sa = Storage.StudentAnnouncementStorage.GetById(studentAnnouncementId);
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(sa.AnnouncementRef);
            if (!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            sa.ExtraCredit = extraCredits;
            sa.Comment = comment;
            sa.State = StudentAnnouncementStateEnum.Manual;
            sa.Dropped = dropped;
            sa.Incomplete = incomplete;
            sa.Late = late;
            sa.Exempt = exempt;
            sa.Absent = absent;

            Storage.StudentAnnouncementStorage.Update(sa);
            if (!string.IsNullOrEmpty(value))
            {
                sa.StiScoreValue = value;
                int number;
                if (int.TryParse(value, out number))
                {
                    //TODO:remove this later
                    var mapper = ServiceLocator.GradingStyleService.GetMapper();
                    sa.GradeValue = gradingStyle.HasValue ? mapper.MapBack(gradingStyle.Value, number)
                                                          : mapper.MapBack(ann.GradingStyle, number);
                }
            }
            var score = new Score { ActivityId = ann.SisActivityId.Value };
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, sa);

            //todo: mock this
            score = ConnectorLocator.ActivityScoreConnector.UpdateScore(score.ActivityId, score.StudentId, score);
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(sa, score);
            var recipientId = sa.PersonRef;
            ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToPerson(sa.AnnouncementRef, recipientId);
            return sa;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            var res = Storage.StudentAnnouncementStorage.GetStudentAnnouncementsDetails(announcementId, Context.UserLocalId ?? 0);
            var notGraded = new List<StudentAnnouncement>();
            if (ann.SisActivityId.HasValue)
            {
                //todo: mock this
                var scores = ConnectorLocator.ActivityScoreConnector.GetSores(ann.SisActivityId.Value);
                foreach (var score in scores)
                {
                    var stAnn = res.FirstOrDefault(x => x.PersonRef == score.StudentId);
                    if (stAnn != null)
                    {
                        stAnn.State = StudentAnnouncementStateEnum.Manual;
                        if (score.NumericScore.HasValue && !stAnn.GradeValue.HasValue)
                            notGraded.Add(stAnn);
                        MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    }
                }
                if (notGraded.Count > 0)
                    Storage.StudentAnnouncementStorage.Update(notGraded);
            }
            return res;
        }

        //TODO: check application existing  
        public StudentAnnouncement SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId)
        {
            var sa = Storage.StudentAnnouncementStorage.GetById(studentAnnouncementId);
            if (sa.State == StudentAnnouncementStateEnum.None)
            {
                sa.GradeValue = value;
                sa.ApplicationRef = applicationId;
                sa.State = StudentAnnouncementStateEnum.Auto;
                Storage.StudentAnnouncementStorage.Update(sa);
            }
            return sa;
        }


        public void ResolveAutoGrading(int announcementId, bool apply)
        {
            if (!BaseSecurity.IsAdminOrTeacher(ServiceLocator.Context))
                throw new ChalkableSecurityException();
            var state = StudentAnnouncementStateEnum.Auto;
            var sas = Storage.StudentAnnouncementStorage.GetList(new StudentAnnouncementShortQuery { AnnouncementId = announcementId, State = state });
            foreach (var studentAnnouncement in sas)
            {
                studentAnnouncement.State = StudentAnnouncementStateEnum.Manual;
                if (!apply)
                    studentAnnouncement.GradeValue = null;
            }
            Storage.StudentAnnouncementStorage.Update(sas);
        }


        public IList<StudentAnnouncementGrade> GetLastGrades(int studentId, int? classId = null, int count = int.MaxValue)
        {
            return Storage.StudentAnnouncementStorage.GetStudentAnnouncementGrades(new StudentAnnouncementQuery
            {
                StudentId = studentId,
                ClassId = classId,
                Count = count
            });
        }
    }
}
