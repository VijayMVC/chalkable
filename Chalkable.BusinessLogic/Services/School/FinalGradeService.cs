using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IFinalGradeService
    {
        IList<FinalGradeAnnouncementType> GetFinalGradeAnnouncementTypes(Guid finalGradeId);
        IList<FinalGradeAnnouncementType> GetFinalGradeAnnouncementTypes(Guid markingPeriodId, Guid classId);
        StudentFinalGrade SetStudentFinalGrade(Guid studentFinalGradeId, int? gradeByAttendance, int? gradeByDiscipline, int? gradeByParticipation, int? teacherGrade, int? adminGrade, string comment);
        
        FinalGradeDetails ReBuildFinalGrade(Guid finalGradeId);
        FinalGradeDetails Update(Guid id, int participation, int attendance, bool dropLowestAttendance, int disipline, bool dropLowestDiscipline, 
                                 GradingStyleEnum gradingStyle, IList<FinalGradeAnnouncementType> byType);
        bool Submit(Guid finalGradeId);
        bool ApproveReject(Guid finalGradeId, bool isApprove);
        
        FinalGradeDetails GetFinalGrade(Guid id, bool needBuildItems = false);
        PaginatedList<FinalGradeDetails> GetPaginatedFinalGrades(FinalGradeStatus status, int start = 0, int count = int.MaxValue);
        IList<ClassDetails> GetFinalizedClasses(Guid markingPeriodId);

    }
    public class FinalGradeService : SchoolServiceBase, IFinalGradeService
    {
        public FinalGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<FinalGradeAnnouncementType> GetFinalGradeAnnouncementTypes(Guid finalGradeId)
        {
            using (var uow = Update())
            {
                var fgDa = new FinalGradeDataAccess(uow);
                if (!fgDa.Exists(finalGradeId))
                {
                    BuildFinalGrade(fgDa, finalGradeId);
                }
                var fgAtda = new FinalGradeAnnouncementTypeDataAccess(uow);
                return fgAtda.GetList(new FinalGradeAnnouncementTypeQuery {FinalGradeId = finalGradeId});
            }
        }

        //TODO: needsTest 
        //TODO: rewrite this
        public IList<FinalGradeAnnouncementType> GetFinalGradeAnnouncementTypes(Guid markingPeriodId, Guid classId)
        {
            var markingPeriodClass = ServiceLocator.MarkingPeriodService.GetMarkingPeriodClass(classId, markingPeriodId);
            if(markingPeriodClass != null)
                return GetFinalGradeAnnouncementTypes(markingPeriodClass.Id);
            return new List<FinalGradeAnnouncementType>();
        }

        public StudentFinalGrade SetStudentFinalGrade(Guid studentFinalGradeId, int? gradeByAttendance, int? gradeByDiscipline,
                                                      int? gradeByParticipation, int? teacherGrade, int? adminGrade, string comment)
        {
            using (var uow = Update())
            {
                var stFgDa = new StudentFinalGradeDataAccess(uow);
                var stFinalGrade = stFgDa.GetById(studentFinalGradeId);
                var finalGrade = new FinalGradeDataAccess(uow).GetVwFinalGrade(stFinalGrade.FinalGradeRef);

                if (!BaseSecurity.IsAdminEditorOrClassTeacher(finalGrade.Class, Context))
                    throw new ChalkableSecurityException();
                
                if (finalGrade.Status == FinalGradeStatus.Approve)
                    throw new ChalkableException(ChlkResources.ERR_FINAL_GRADE_IS_APPROVE);

                stFinalGrade.GradeByAttendance = gradeByAttendance ?? 0;
                stFinalGrade.GradeByDiscipline = gradeByDiscipline ?? 0;
                stFinalGrade.GradeByParticipation = gradeByParticipation ?? 0;
                stFinalGrade.Comment = comment;

                var mapper = ServiceLocator.GradingStyleService.GetMapper();
                if (Context.Role == CoreRoles.TEACHER_ROLE)
                    stFinalGrade.TeacherGrade = mapper.MapBack(finalGrade.GradingStyle, teacherGrade);
                if (BaseSecurity.IsAdminViewer(Context))
                    stFinalGrade.AdminGrade = mapper.MapBack(finalGrade.GradingStyle, adminGrade); 
               
                stFgDa.Update(stFinalGrade);
                uow.Commit();
                return stFinalGrade;
            }

        }

        
        public FinalGradeDetails ReBuildFinalGrade(Guid finalGradeId)
        {
            var finalGrade = GetFinalGradeDetails(finalGradeId);
            if (finalGrade == null || finalGrade.Status == FinalGradeStatus.Open)
            {
               return BuildFinalGrade(finalGradeId);
            }
            return finalGrade;
        }
        public FinalGradeDetails Update(Guid id, int participation, int attendance, bool dropLowestAttendance, int disipline,
                                        bool dropLowestDiscipline, GradingStyleEnum gradingStyle, IList<FinalGradeAnnouncementType> byType)
        {
            using (var uow = Update())
            {
                var finalGrade = GetFinalGradeDetails(id);
                if (!BaseSecurity.IsAdminEditorOrClassTeacher(finalGrade.Class, Context))
                    throw new ChalkableSecurityException();
                if (gradingStyle != GradingStyleEnum.Abcf && gradingStyle != GradingStyleEnum.Numeric100)
                    throw new ChalkableException(ChlkResources.ERR_FINAL_GRADE_INVALID_GRADING_STYLE);
                var allPercent = participation + attendance + disipline + byType.Sum(x => x.PercentValue);
                if (allPercent != 100)
                    throw new ChalkableException(ChlkResources.ERR_FINAL_GRADE_INVALID_PERCENTS_SUM);

                finalGrade.ParticipationPercent = participation;
                finalGrade.Attendance = attendance;
                finalGrade.DropLowestAttendance = dropLowestAttendance;
                finalGrade.Discipline = disipline;
                finalGrade.DropLowestDiscipline = dropLowestDiscipline;
                finalGrade.GradingStyle = gradingStyle;

                var fgAnnTypeDa = new FinalGradeAnnouncementTypeDataAccess(uow);
                var byTypeExisting = fgAnnTypeDa.GetList(new FinalGradeAnnouncementTypeQuery { FinalGradeId = id });
                foreach (var finalGradeAnnouncementType in byTypeExisting)
                {
                    var t = byType.FirstOrDefault(x => x.Id == finalGradeAnnouncementType.Id);
                    if (t != null)
                    {
                        finalGradeAnnouncementType.PercentValue = t.PercentValue;
                        finalGradeAnnouncementType.GradingStyle = t.GradingStyle;
                        finalGradeAnnouncementType.DropLowest = t.DropLowest;
                    }
                }
                fgAnnTypeDa.Update(byTypeExisting);

                finalGrade.FinalGradeAnnouncementTypes = byTypeExisting;
                new FinalGradeDataAccess(uow).Update(finalGrade);

                finalGrade = CalcGradeByAnnouncements(uow, finalGrade);
                uow.Commit();
                return finalGrade;
            }
        }

        public bool Submit(Guid finalGradeId)
        {
            using (var uow = Update())
            {
                var da = new FinalGradeDataAccess(uow);
                var finalGrade = da.GetVwFinalGrade(finalGradeId);
                if (!BaseSecurity.IsAdminEditorOrClassTeacher(finalGrade.Class, Context))
                    throw new ChalkableSecurityException();
                bool isOpen = finalGrade.Status == FinalGradeStatus.Open;
                if (isOpen)
                {
                    finalGrade.Status = FinalGradeStatus.Submit;
                    da.Update(finalGrade);
                }
                uow.Commit();
                return isOpen;
            }
        }
        public bool ApproveReject(Guid finalGradeId, bool isApprove)
        {
            if (!BaseSecurity.IsAdminGrader(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new FinalGradeDataAccess(uow);
                var finalGrade = da.GetById(finalGradeId);
                var isSubmitted = finalGrade.Status == FinalGradeStatus.Submit;
                if (isSubmitted)
                {
                    finalGrade.Status = isApprove ? FinalGradeStatus.Approve : FinalGradeStatus.Open;
                    da.Update(finalGrade);
                }
                uow.Commit();
                return isSubmitted;
            }
        }
      
        public FinalGradeDetails GetFinalGrade(Guid id, bool needBuildItems = false)
        {
            var finalGrade = GetFinalGradeDetails(id);
            if (finalGrade != null)
            {
                if (needBuildItems)
                {
                    finalGrade = CalcGradeByAnnouncements(finalGrade);
                }
                return finalGrade;
            }
            return BuildFinalGrade(id);
        }
        public PaginatedList<FinalGradeDetails> GetPaginatedFinalGrades(FinalGradeStatus status, int start = 0, int count = int.MaxValue)
        {
            return GetPaginatedFinalGrades(new FinalGradeQuery {Status = status}, start, count);
        }

        private FinalGradeDetails GetFinalGradeDetails(Guid id)
        {
            return GetFinalGradeQueryResult(new FinalGradeQuery {Start = 0, Count = 1, Id = id})
                    .FinalGrades.FirstOrDefault();
        }
        private FinalGradeQueryResult GetFinalGradeQueryResult(FinalGradeQuery query)
        {
            using (var uow = Read())
            {
                query.CallerId = Context.UserId;
                query.CallerRoleId = Context.Role.Id;
                return new FinalGradeDataAccess(uow).GetFinalGradesDetails(query);
            }
        }
        private PaginatedList<FinalGradeDetails> GetPaginatedFinalGrades(FinalGradeQuery query, int start, int count)
        {
            var res = GetFinalGradeQueryResult(query);
            query.Start = start;
            query.Count = count;
            return new PaginatedList<FinalGradeDetails>(res.FinalGrades, query.Start / count, count, res.SourceCount);
        }

        private FinalGradeDetails BuildFinalGrade(Guid markingPeriodClassId)
        {
            using (var uow = Update())
            {
                var res = BuildFinalGrade(new FinalGradeDataAccess(uow), markingPeriodClassId);
                uow.Commit();
                return res;
            }
        }
        private FinalGradeDetails BuildFinalGrade(FinalGradeDataAccess dataAccess, Guid markingPeriodClassId)
        {
            var res = dataAccess.BuildFinalGrade(markingPeriodClassId, Context.UserId, Context.Role.Id);
            if (res == null)
                throw new ChalkableSecurityException();
            return res;
        }
   
        private FinalGradeDetails CalcGradeByAnnouncements(FinalGradeDetails finalGrade)
        {
            using (var uow = Update())
            {
                var res = CalcGradeByAnnouncements(uow, finalGrade);
                uow.Commit();
                return res;
            }
        }
        private FinalGradeDetails CalcGradeByAnnouncements(UnitOfWork uow, FinalGradeDetails finalGrade)
        {
            if (!BaseSecurity.IsAdminEditorOrClassTeacher(finalGrade.Class, Context))
                throw new ChalkableSecurityException();
            if (finalGrade == null || finalGrade.Status != (int)FinalGradeStatus.Open)//TODO: add typed property
                throw new ChalkableException(ChlkResources.ERR_FINAL_GRADE_INVALID_STATUS);
            var stFinalGrades =  new StudentFinalGradeDataAccess(uow).ReCalculateGradeByAnnouncement(finalGrade.Id);
            finalGrade.StudentFinalGrades = stFinalGrades;
            return finalGrade;
        }



        //TODO: needs tests 
        public IList<ClassDetails> GetFinalizedClasses(Guid markingPeriodId)
        {
            var finalGrades = GetFinalGradeQueryResult(new FinalGradeQuery {MarkingPeriodId = markingPeriodId}).FinalGrades;
            return finalGrades.Where(x=>x.Status != FinalGradeStatus.Open).Select(x => x.Class).ToList();
        }
    }
}
