using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class CourseInfoDataAccess : DataAccessBase
    {
        public CourseInfoDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(CourseInfo courseInfo)
        {
            SimpleInsert(courseInfo);
        }
        public void Update(CourseInfo courseInfo)
        {
            SimpleUpdate(courseInfo);
        }
        public void Delete(Guid id)
        {
            SimpleDelete<CourseInfo>(id);
        }
        public CourseInfo GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<CourseInfo>(conds);
        }
        public PaginatedList<CourseInfo> GetCourses(int start, int count)
        {
           return PaginatedSelect<CourseInfo>("Id", start, count);
        }
    }
}
