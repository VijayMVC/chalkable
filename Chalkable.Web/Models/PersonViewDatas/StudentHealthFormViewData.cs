using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentHealthFormViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentId { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public static IList<StudentHealthFormViewData> Create(IList<StudentHealthFormInfo> studentHealthForms)
        {
            return studentHealthForms.Select(x => new StudentHealthFormViewData
            {
                Id = x.Id,
                Name = x.Name,
                StudentId = x.StudentId,
                VerifiedDate = x.VerifiedDate
            }).ToList();
        }
    }
}