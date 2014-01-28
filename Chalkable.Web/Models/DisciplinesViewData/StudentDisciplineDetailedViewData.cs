using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class StudentDisciplineDetailedViewData : ShortPersonViewData
    {
        public IList<StudentDisciplineBoxViewData> DisciplineBoxes { get; set; }
        public MarkingPeriodViewData MarkingPeriod { get; set; }
        public string Summary { get; set; }

        protected StudentDisciplineDetailedViewData(PersonDetails student) : base(student) { }

        //TODO: refactor this code later 
        public static StudentDisciplineDetailedViewData Create(PersonDetails student, IList<ClassDisciplineDetails> disciplines, MarkingPeriod markingPeriod)
        {
            var classdiscTypes = new List<ClassDisciplineTypeDetails>();
            foreach (var discipline in disciplines)
            {
                classdiscTypes.AddRange(discipline.DisciplineTypes);
            } 
            var disciplinesDictionary = classdiscTypes.OrderBy(x => x.DisciplineType.Name)
                                                      .GroupBy(x => x.DisciplineTypeRef)
                                                      .ToDictionary(x => x.Key, y => y.ToList());
           
            string summary = string.Format("{0} is doing great. No discipline issues.", student.FirstName);
            string className;
            if(disciplinesDictionary.Count > 0)
            {
                var classesDictionary = disciplines.GroupBy(x => x.Class.Id).ToDictionary(x => x.Key, x => x.ToList());
                if (classesDictionary.Count > 0)
                {
                    var classDisciplines = classesDictionary.OrderByDescending(x => x.Value.Count).First().Value;
                    className = classDisciplines.First().Class.Name;
                    summary = string.Format(ChlkResources.STUDENT_IS_HAVING_DISCIPLINARY_ISSUES, student.FirstName, className);
                }
            }
            var disciplineBoxes = new List<StudentDisciplineBoxViewData>();
            foreach (var disciplinesList in disciplinesDictionary)
            {
                disciplineBoxes.Add(StudentDisciplineBoxViewData.Create(disciplinesList.Value.Count, disciplinesList.Value.Select(x=>x.ClassDiscipline).ToList(), disciplinesList.Value.First().DisciplineType));
            }
            var res = new StudentDisciplineDetailedViewData(student)
                          {
                              MarkingPeriod = MarkingPeriodViewData.Create(markingPeriod),
                              Summary = summary,
                              DisciplineBoxes = disciplineBoxes
                          };
            return res;
        }
    }


    public class StudentDisciplineBoxViewData : HoverBoxesViewData<StudentDisciplineHoverViewData>
    {
        public string Name { get; set; }
        public bool IsPassing { get; set; }
        public static StudentDisciplineBoxViewData Create(int typeCount, List<ClassDisciplineDetails> disciplines, DisciplineType disciplineType)
        {
            var isPassing = false; //TODO: calc when is passing
            var disciplinesDictionary = disciplines.OrderBy(x => x.Class.Name).GroupBy(x => x.Class.Id).ToDictionary(x => x.Key, x => x.ToList());

            return new StudentDisciplineBoxViewData
            {
                Hover = StudentDisciplineHoverViewData.Create(disciplinesDictionary),
                IsPassing = isPassing,
                Title = typeCount.ToString(),
                Name = disciplineType.Name
            };
        }
    }

    public class StudentDisciplineHoverViewData
    {
        public int Value { get; set; }
        public string ClassName { get; set; }
        public const int HOVER_DATA_COUNT = 4;
        private const string OTHER = "Other";

        public static StudentDisciplineHoverViewData Create(List<ClassDisciplineDetails> disciplines)
        {

            var res = new StudentDisciplineHoverViewData
            {
                Value = disciplines.Sum(x=>x.DisciplineTypes.Count),
                ClassName = disciplines.First().Class.Name
            };
            return res;
        }
        public static IList<StudentDisciplineHoverViewData> Create(Dictionary<int, List<ClassDisciplineDetails>> disciplinesDictionary)
        {
            var res = disciplinesDictionary.Select(x => Create(x.Value)).OrderByDescending(x => x.Value)
                .ToList();
            if (res.Count > HOVER_DATA_COUNT)
            {
                var rest = res.Skip(HOVER_DATA_COUNT).Sum(x => x.Value);

                res = res.Take(HOVER_DATA_COUNT).ToList();
                res.Add(new StudentDisciplineHoverViewData
                {
                    ClassName = OTHER,
                    Value = rest
                });
            }
            return res;
        }
    }

}