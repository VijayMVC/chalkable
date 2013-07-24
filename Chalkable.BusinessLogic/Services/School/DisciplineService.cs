using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDisciplineService
    {

        IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<Guid> gradeLevelIds = null);
        IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, Guid personId, DateTime start, DateTime end, bool needsAllData = false);
    }

    public class DisciplineService : SchoolServiceBase, IDisciplineService
    {
        public DisciplineService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }



        //TODO: needs test... security
        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(ClassDisciplineQuery query, IList<Guid> gradeLevelIds = null)
        {
            using (var uow = Read())
            {
                var res = new ClassDisciplineDataAccess(uow).GetClassDisciplinesDetails(query);
                if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                    res = res.Where(x => gradeLevelIds.Contains(x.Class.GradeLevelRef)).ToList();
                return res;
            }
        }

        public IList<ClassDisciplineDetails> GetClassDisciplineDetails(Guid schoolYearId, Guid personId, DateTime start, DateTime end, bool needsAllData = false)
        {
            return GetClassDisciplineDetails(new ClassDisciplineQuery
                {
                    SchoolYearId = schoolYearId,
                    PersonId = personId,
                    FromDate = start,
                    ToDate = end,
                    NeedAllData = needsAllData
                });
        }
    }
}
