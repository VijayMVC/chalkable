using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class StandardSubjectViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static StandardSubjectViewData Create(StandardSubject standardSubject)
        {
            return new StandardSubjectViewData
                {
                    Id = standardSubject.Id,
                    Name = standardSubject.Name,
                    Description = standardSubject.Description
                };
        }
        public static IList<StandardSubjectViewData> Create(IList<StandardSubject> standardSubjects)
        {
            return standardSubjects.Select(Create).ToList();
        } 
    }
}