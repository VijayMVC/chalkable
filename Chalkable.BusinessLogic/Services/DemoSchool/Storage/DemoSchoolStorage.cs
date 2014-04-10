using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolStorage:BaseDemoStorage<int, Data.School.Model.School>
    {
        public DemoSchoolStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(Data.School.Model.School school)
        {

            if (!data.ContainsKey(school.Id))
                data[school.Id] = school;
        }

        public void Update(Data.School.Model.School school)
        {
            if (data.ContainsKey(school.Id))
                data[school.Id] = school;
        }

        public void Add(IList<Data.School.Model.School> schools)
        {
            foreach (var school in schools)
            {
                Add(school);
            }
        }

        public void Update(IList<Data.School.Model.School> schools)
        {
            foreach (var school in schools)
            {
                Update(school);
            }
        }
      
    }
}
