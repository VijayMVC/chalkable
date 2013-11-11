using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class StudentDisciplineSummaryViewData
    {
        public PersonViewData Student { get; set; }
        public int Total { get; set; }
        public int DisciplineRecordsNumber { get; set; }
        public string Summary { get; set; }

        protected StudentDisciplineSummaryViewData(Person student)
        {
            Student = PersonViewData.Create(student);
        }

        public static IList<StudentDisciplineSummaryViewData> Create(IList<ClassDisciplineDetails> disciplines)
        {
            throw new NotImplementedException();
            //IDictionary<Guid, List<ClassDisciplineTypeDetails>> stDiscTypeDic = new Dictionary<Guid, List<ClassDisciplineTypeDetails>>();
            //IDictionary<Guid, Person> stDic = new Dictionary<Guid, Person>();
            //foreach (var discipline in disciplines)
            //{
            //    if (!stDiscTypeDic.ContainsKey(discipline.Student.Id))
            //    {
            //        stDic.Add(discipline.Student.Id, discipline.Student);
            //        stDiscTypeDic.Add(discipline.Student.Id, new List<ClassDisciplineTypeDetails>());
            //    }
            //    stDiscTypeDic[discipline.Student.Id].AddRange(discipline.DisciplineTypes);
            //}
            //return stDiscTypeDic.Select(x => new StudentDisciplineSummaryViewData(stDic[x.Key])
            //{
            //    Summary = BuildSummary(x.Value),
            //    Total = x.Value.Sum(y => y.DisciplineType.Score),
            //    DisciplineRecordsNumber = x.Value.Count
            //}).ToList();
        }

        private static string BuildSummary(IEnumerable<ClassDisciplineTypeDetails> disciplineTypes)
        {
            var dic = disciplineTypes.GroupBy(x => x.DisciplineType.Name).ToDictionary(x => x.Key, x => x.Count());
            return dic.Select(x => string.Format("{0} {1}", x.Key, x.Value)).JoinString(",");
        }
    }
}