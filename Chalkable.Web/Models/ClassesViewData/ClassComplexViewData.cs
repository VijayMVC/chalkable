using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassComplexViewData : ClassViewData
    {
        public Room Room { get; set; }
        public IList<string> Periods { get; set; }
        public IList<string> DayTypes { get; set; }

        public ClassComplexViewData(ClassDetails classComplex) : base(classComplex)
        {
            if (classComplex.PrimaryTeacher != null)
            {
                Teacher.DisplayName = classComplex.PrimaryTeacher.FullName(false, true);
            }
        }

        public static IList<ClassComplexViewData> Create(IList<ClassDetails> classDetails, IList<Room> rooms, IList<DayType> dayTypes)
        {
            return classDetails.Select(x => new ClassComplexViewData(x)
            {
                Room = x.RoomRef.HasValue ? rooms.FirstOrDefault(r => r.Id == x.RoomRef.Value) : null,
                Periods = x.ClassPeriods?.Select(cp => cp.Period.Name).Distinct().ToList(),
                DayTypes = dayTypes.Where(dt => x.ClassPeriods.Select(cp => cp.DayTypeRef).Contains(dt.Id)).Select(dq => dq.Name).Distinct().ToList()
        }).ToList();
        }
    }
}