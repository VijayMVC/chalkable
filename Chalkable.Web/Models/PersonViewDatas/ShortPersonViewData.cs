using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class ShortPersonViewData
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public RoleViewData Role { get; set; }
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public bool? IsWithDrawn { get; set; }

        protected ShortPersonViewData(Person person)
        {
            Id = person.Id;
            DisplayName = person.RoleRef == CoreRoles.STUDENT_ROLE.Id ? person.FullName : person.ShortSalutationName; //TODO: think about this
            FullName = person.FullName;
            FirstName = person.CapitilizedFirstName;
            LastName = person.CapitilizedLastName;
            Gender = person.Gender;
            if (person.RoleRef > 0)
            {
                Role = RoleViewData.Create(CoreRoles.GetById(person.RoleRef));
                if (person.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                {
                    HasMedicalAlert = person.HasMedicalAlert;
                    IsAllowedInetAccess = person.IsAllowedInetAccess;
                    SpecialInstructions = person.SpecialInstructions;
                    SpEdStatus = person.SpEdStatus;    
                }
            }
            IsWithDrawn = person.IsWithdrawn;

        }
        public static ShortPersonViewData Create(Person person)
        {
            return new ShortPersonViewData(person);
        }
        public static IList<ShortPersonViewData> Create(IList<Person> persons)
        {
            return persons.Select(Create).ToList();
        }
    }
}