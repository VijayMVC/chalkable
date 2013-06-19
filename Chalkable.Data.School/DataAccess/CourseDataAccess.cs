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
    public class CourseDataAccess : DataAccessBase
    {
        public CourseDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Course courseInfo)
        {
            SimpleInsert(courseInfo);
        }
        public void Update(Course courseInfo)
        {
            SimpleUpdate(courseInfo);
        }
        public void Delete(Guid id)
        {
            SimpleDelete<Course>(id);
        }
        public Course GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<Course>(conds);
        }
        public PaginatedList<Course> GetCourses(int start, int count)
        {
           return PaginatedSelect<Course>("Id", start, count);
        }
    }
}
