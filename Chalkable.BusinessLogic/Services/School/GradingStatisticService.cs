using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IGradingStatisticService
    {
        IList<StudentGradeAvgPerMPC> GetStudentGradePerMPC(Guid teacherId, IList<Guid> markingPeriodIds);
        IList<StudentGradeAvgPerClass> GetStudentGradePerClass(Guid teacherId, Guid schoolYearId);
        IList<MarkingPeriodClassGradeAvg> GetClassGradeAvgPerMP(Guid classId, Guid schoolYearId, List<Guid> markingPeriodIds, Guid? teacherId);
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
            return query;
        }

        public IList<StudentGradeAvgPerMPC> GetStudentGradePerMPC(Guid teacherId, IList<Guid> markingPeriodIds)
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

        public IList<StudentGradeAvgPerClass> GetStudentGradePerClass(Guid teacherId, Guid schoolYearId)
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
    }
}
