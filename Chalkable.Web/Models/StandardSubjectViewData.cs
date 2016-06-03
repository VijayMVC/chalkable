using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class StandardSubjectViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public static StandardSubjectViewData Create(StandardSubject standardSubject)
        {
            return new StandardSubjectViewData
                {
                    Id = standardSubject.Id,
                    Name = standardSubject.Name,
                    DisplayName = standardSubject.AdoptionYear == null ? standardSubject.Name : standardSubject.Name + " (" + standardSubject.AdoptionYear + ")",  
                    Description = standardSubject.Description
                };
        }
        public static IList<StandardSubjectViewData> Create(IList<StandardSubject> standardSubjects)
        {
            return standardSubjects.Select(Create).ToList();
        } 
    }
}