using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradeLevelStorage:BaseDemoIntStorage<GradeLevel>
    {
        public DemoGradeLevelStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<GradeLevel> GetGradeLevels(int? schoolId)
        {
            var schoolGradeLevels = Storage.SchoolGradeLevelStorage.GetAll(schoolId).Select(x => x.GradeLevelRef).ToList();
            return data.Where(x => schoolGradeLevels.Contains(x.Value.Id)).Select(x => x.Value).ToList();
        }

        public override void Setup()
        {
            for (var lvl = DemoSchoolConstants.GradeLevel1; lvl <= DemoSchoolConstants.GradeLevel12; ++lvl)
            {
                Add(new GradeLevel
                {
                    Description = "",
                    Id = lvl,
                    Name = lvl.ToString(CultureInfo.InvariantCulture),
                    Number = lvl
                });
            }
        }
    }
}
