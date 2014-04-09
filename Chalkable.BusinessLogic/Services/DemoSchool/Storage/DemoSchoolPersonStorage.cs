using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolPersonStorage:BaseDemoStorage<int, SchoolPerson>
    {
        public DemoSchoolPersonStorage(DemoStorage storage) : base(storage)
        {

        }

        public SchoolPerson GetSchoolPerson(int personId, int? schoolLocalId, int? roleId)
        {
            var persons = data.Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolLocalId);
            if (roleId.HasValue)
                persons = data.Where(x => x.Value.RoleRef == roleId);
            return persons.Select(x => x.Value).First();
        }

        public void Add(IList<SchoolPerson> assignments)
        {
            foreach (var schoolPerson in assignments)
            {
                data.Add(GetNextFreeId(), schoolPerson);
            }
        }

        public bool Exists(int personId, int roleRef, int? schoolRef)
        {
            return
                data.Count(
                    x => x.Value.RoleRef == roleRef && x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef) ==
                1;
        }

        public int GetRoleId(int personId, int schoolRef)
        {
            return
                data.Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef)
                    .Select(x => x.Value.RoleRef).First();
        }

        public void Delete(IList<SchoolPerson> schoolPersons)
        {
            foreach (var schoolPerson in schoolPersons)
            {
                var item =
                    data.First(x => x.Value.PersonRef == schoolPerson.PersonRef && x.Value.RoleRef == schoolPerson.RoleRef &&
                                    x.Value.SchoolRef == schoolPerson.SchoolRef);

                Delete(item.Key);
            }
        }

        public void Setup()
        {
            var sps = new List<SchoolPerson>
            {
                new SchoolPerson
                {
                    PersonRef = 1197,
                    RoleRef = 5,
                    SchoolRef = 1
                },
                new SchoolPerson
                {
                    PersonRef = 1196,
                    RoleRef = 3,
                    SchoolRef = 1
                },
                new SchoolPerson
                {
                    PersonRef = 1195,
                    RoleRef = 2,
                    SchoolRef = 1
                }
            };

            Add(sps);
        }
    }
}
