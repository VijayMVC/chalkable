using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassPeriodDataAccess : DataAccessBase
    {
        public ClassPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void Delete(Guid id)
        {
            SimpleDelete(new Dictionary<string, object> {{"Id", id}});
        }

    }

    public class ClassPeriodQuery
    {
        public Guid MarkingPeriodId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? PeriodId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? SectionId { get; set; }
        public int? Time { get; set; }
    }
}
