using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardSubjectStorage
    {
        private Dictionary<int ,StandardSubject> standardSubjectsData = new Dictionary<int, StandardSubject>();
 
        public void Add(IList<StandardSubject> standardSubjects)
        {
            foreach (var standardSubject in standardSubjects)
            {
                var subject = standardSubjectsData.FirstOrDefault(x => x.Key == standardSubject.Id).Value;
                if (subject == null)
                {
                    standardSubjectsData.Add(standardSubject.Id, standardSubject);
                }
            }
        }

        public IList<StandardSubject> GetAll()
        {
            return standardSubjectsData.Select(x => x.Value).ToList();
        }

        public void Update(IList<StandardSubject> standardSubjects)
        {
            foreach (var standardSubject in standardSubjects)
            {
                var subject = standardSubjectsData.FirstOrDefault(x => x.Key == standardSubject.Id).Value;
                if (subject != null)
                {
                    standardSubjectsData[standardSubject.Id] = standardSubject;
                }
            }
        }
    }
}
