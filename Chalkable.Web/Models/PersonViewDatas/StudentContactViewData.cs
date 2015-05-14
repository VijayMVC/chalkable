using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentContactViewData 
    {
        public PersonInfoViewData PersonInfo { get; set; }
        public string RelationshipName { get; set; }
        public bool IsFamalyMember { get; set; }

        public static IList<StudentContactViewData> Create(IList<StudentContactDetails> studentContacts)
        {
            return studentContacts.Select(x => new StudentContactViewData
                {
                    PersonInfo = PersonInfoViewData.Create(x.Person),
                    RelationshipName = x.ContactRelationship.Name,
                    IsFamalyMember = x.IsFamilyMember
                }).ToList();
        } 
    }
}