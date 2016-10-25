using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class StudentHealthFormInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public static IList<StudentHealthFormInfo> Create(IList<StudentHealthForm> studentHealthForms)
        {
            return studentHealthForms.Select(x => new StudentHealthFormInfo
            {
                Id = x.Id,
                Name = x.Name,
                StudentId = x.StudentId,
                VerifiedDate = x.VerifiedDate
            }).ToList();
        } 
    }
}
