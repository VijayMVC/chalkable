using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceDetailedViewData : ShortPersonViewData
    {

        public StudentAttendanceBoxViewData AbsentSection { get; set; }
        public StudentAttendanceBoxViewData LateSection { get; set; }
        public StudentAttendanceBoxViewData ExcusedSection { get; set; }
        public StudentAttendanceBoxViewData PresentSection { get; set; }
        public MarkingPeriodViewData MarkingPeriod { get; set; }


        protected StudentAttendanceDetailedViewData(Person person) : base(person)
        {
        }

        public static StudentAttendanceDetailedViewData Create(Person student, 
            IList<ClassAttendanceDetails> studentAttendnaces, MarkingPeriod markingPeriod)
        {
            var attendancesDictionary = studentAttendnaces.OrderBy(x => x.Class.Name).GroupBy(x => x.Class.Id).ToDictionary(x => x.Key, x => x.ToList());
            var res = new StudentAttendanceDetailedViewData(student)
            {
                MarkingPeriod = MarkingPeriodViewData.Create(markingPeriod),
                AbsentSection = StudentAttendanceBoxViewData.Create(studentAttendnaces, attendancesDictionary, AttendanceTypeEnum.Absent),
                LateSection = StudentAttendanceBoxViewData.Create(studentAttendnaces, attendancesDictionary, AttendanceTypeEnum.Late),
                ExcusedSection = StudentAttendanceBoxViewData.Create(studentAttendnaces, attendancesDictionary, AttendanceTypeEnum.Excused),
                PresentSection = StudentAttendanceBoxViewData.Create(studentAttendnaces, attendancesDictionary, AttendanceTypeEnum.Present)
            };
            return res;
        }
    }

    public class StudentAttendanceBoxViewData : HoverBoxesViewData<StudentAttendanceHoverViewData>
    {
        public bool IsPassing { get; set; }

        public static StudentAttendanceBoxViewData Create(IList<ClassAttendanceDetails> attendances, 
            IDictionary<Guid, List<ClassAttendanceDetails>> attendancesDictionary, AttendanceTypeEnum type)
        {
            var isPassing = true;
            var typeCount = attendances.Count(x => x.Type == type);

            if (type == AttendanceTypeEnum.Absent || type == AttendanceTypeEnum.Late)
            {
                var presentCount = attendances.Count(x => x.Type == AttendanceTypeEnum.Present);
                if (presentCount == 0) presentCount = 1;
                isPassing = 100 * typeCount / presentCount < (type == AttendanceTypeEnum.Absent ? 10 : 5);
            }

            return new StudentAttendanceBoxViewData
            {
                Hover = StudentAttendanceHoverViewData.Create(attendancesDictionary, type),
                IsPassing = isPassing,
                Title = typeCount.ToString()
            };
        }
    }

    public class StudentAttendanceHoverViewData
    {
        public int Value { get; set; }
        public string ClassName { get; set; }
        public const int HOVER_DATA_COUNT = 4;
        public static StudentAttendanceHoverViewData Create(List<ClassAttendanceDetails> attendanceComplexes, AttendanceTypeEnum type)
        {
            var res = new StudentAttendanceHoverViewData
            {
                Value = attendanceComplexes.Count(x => x.Type == type),
                ClassName = attendanceComplexes.First().Class.Name
            };
            return res;
        }

        private const string OTHER = "Other";
        public static IList<StudentAttendanceHoverViewData> Create(IDictionary<Guid, List<ClassAttendanceDetails>> attendancesDictionary, AttendanceTypeEnum type)
        {
            var res = attendancesDictionary.Select(x => Create(x.Value, type))
                .Where(x => x.Value > 0)
                .OrderByDescending(x => x.Value)
                .ToList();

            if (res.Count > HOVER_DATA_COUNT)
            {
                var rest = res.Skip(HOVER_DATA_COUNT).Sum(x => x.Value);

                res = res.Take(HOVER_DATA_COUNT).ToList();
                res.Add(new StudentAttendanceHoverViewData
                {
                    ClassName = OTHER,
                    Value = rest
                });
            }
            return res;
        }
    }


}