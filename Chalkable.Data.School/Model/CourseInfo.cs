using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class CourseInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public Guid ChalkableDepartmentRef { get; set; }
        public int? SisId { get; set; }
    }
}
