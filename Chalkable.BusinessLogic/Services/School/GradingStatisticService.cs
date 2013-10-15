using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStatisticService
    {
        IList<StudentGradeAvgPerMPC> GetStudentsGradePerMPC(Guid teacherId, IList<Guid> markingPeriodIds);
        IList<StudentGradeAvgPerClass> GetStudentsGradePerClass(Guid teacherId, Guid schoolYearId);
        IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(Guid classId, Guid schoolYearId, List<Guid> markingPeriodIds, Guid? teacherId);
        IList<StudentGradeAvgPerDate> GetStudentGradePerDate(Guid studentId, Guid markingPeriodId, Guid? classId);
        IList<StudentClassGradeStats> GetStudentClassGradeStats(Guid markingPeriodId, Guid classId, Guid? studentId);
        IList<DepartmentGradeAvg> GetDepartmentGradeAvgPerMp(Guid markingPeriodId, IList<Guid> gradeLevelIds);
        IList<ClassPersonGradingStats> GetFullGradingStats(Guid markingPeriodId, Guid studentId);
        IList<StudentGradingRank> GetStudentGradingRanks(Guid schoolYearId, Guid? studentId, Guid? gradeLevelId, Guid? classId);
    }
    public class GradingStatisticService : SchoolServiceBase, IGradingStatisticService
    {
        public GradingStatisticService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        //TODO: needs test 
        private GradingStatisticQuery QueryFiltering(GradingStatisticQuery query)
        {
            query.CallerId = Context.UserId;
            query.Role = Context.Role.Id;
            query.Date = Context.NowSchoolTime.Date;
            return query;
        }

        public IList<StudentGradeAvgPerMPC> GetStudentsGradePerMPC(Guid teacherId, IList<Guid> markingPeriodIds)
        {
            using (var uow = Read())
            {
                var query = QueryFiltering(new GradingStatisticQuery
                    {
                        TeacherId = teacherId,
                        MarkingPeriodIds = markingPeriodIds
                    });
                return new GradingStatisticDataAccess(uow).CalcStudentGradeAvgPerMPC(query);
            }
        }

        public IList<StudentGradeAvgPerClass> GetStudentsGradePerClass(Guid teacherId, Guid schoolYearId)
        {
            using (var uow = Read())
            {
                var query = QueryFiltering(new GradingStatisticQuery
                    {
                        TeacherId = teacherId,
                        SchoolYearId = schoolYearId
                    });
                return new GradingStatisticDataAccess(uow).CalcStudentGradeAvgPerClass(query);
            }
        }

        public IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(Guid classId, Guid schoolYearId, List<Guid> markingPeriodIds, Guid? teacherId)
        {
            using (var uow = Read())
            {
                var query = QueryFiltering(new GradingStatisticQuery
                    {
                        ClassId = classId,
                        SchoolYearId = schoolYearId,
                        MarkingPeriodIds = markingPeriodIds,
                        TeacherId = teacherId
                    });
                return new GradingStatisticDataAccess(uow).CalcClassGradingPerMp(query);
            }
        }

        public IList<StudentGradeAvgPerDate> GetStudentGradePerDate(Guid studentId, Guid markingPeriodId, Guid? classId)
        {
            using (var uow = Read())
            {
                return   new GradingStatisticDataAccess(uow).CalcStudentGradeStatsPerDate(studentId, markingPeriodId, classId, 7);
            }
        }

        public IList<StudentClassGradeStats> GetStudentClassGradeStats(Guid markingPeriodId, Guid classId, Guid? studentId)
        {
            using (var uow = Read())
            {
                return new GradingStatisticDataAccess(uow).CalcStudentClassGradeStats(classId, markingPeriodId, studentId, 7);
            }
        }

        public IList<DepartmentGradeAvg> GetDepartmentGradeAvgPerMp(Guid markingPeriodId, IList<Guid> gradeLevelIds)
        {
            using (var uow = Read())
            {
                return new GradingStatisticDataAccess(uow).CalcDepartmentGradeAvgPerMp(markingPeriodId, Context.UserId,
                                                                                       Context.Role.Id, gradeLevelIds);
            }
        }


        public IList<ClassPersonGradingStats> GetFullGradingStats(Guid markingPeriodId, Guid studentId)
        {
            using (var uow = Read())
            {
                return new GradingStatisticDataAccess(uow).CalcGradingStats(Context.UserId, Context.Role.Id, studentId, markingPeriodId);
            }
        }

        public IList<StudentGradingRank> GetStudentGradingRanks(Guid schoolYearId, Guid? studentId, Guid? gradeLevelId, Guid? classId)
        {
            using (var uow = Read())
            {
                return new GradingStatisticDataAccess(uow).GetStudentGradingRank(Context.UserId, Context.Role.Id, schoolYearId, gradeLevelId, studentId, classId);
            }
        }
    }
}
