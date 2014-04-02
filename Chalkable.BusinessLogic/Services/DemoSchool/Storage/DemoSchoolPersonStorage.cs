using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolPersonStorage:BaseDemoStorage<int, SchoolPerson>
    {
        private int index = 0;
        public DemoSchoolPersonStorage(DemoStorage storage) : base(storage)
        {

        }

        public object GetSchoolPerson(int toPersonId, int? schoolLocalId, int? o)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<SchoolPerson> assignments)
        {
            foreach (var schoolPerson in assignments)
            {
                data.Add(index++, schoolPerson);
            }
        }

        public bool Exists(int teacherId, int id, int? schoolRef)
        {
            throw new System.NotImplementedException();
        }

        public int GetRoleId(int personId, int schoolRef)
        {
            return
                data.Where(x => x.Value.PersonRef == personId && x.Value.SchoolRef == schoolRef)
                    .Select(x => x.Value.RoleRef).First();
        }
    }
}
