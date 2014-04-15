using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassStandardStorage:BaseDemoStorage<int, ClassStandard>
    {
        public DemoClassStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<ClassStandard> Add(IList<ClassStandard> classStandards)
        {
            foreach (var classStandard in classStandards)
            {
                data.Add(GetNextFreeId(), classStandard);
            }
            return classStandards;
        }

        public void Delete(IList<ClassStandard> classStandards)
        {
            foreach (var classStandard in classStandards)
            {
                var item =
                    data.First(
                        x =>
                            x.Value.ClassRef == classStandard.ClassRef &&
                            x.Value.StandardRef == classStandard.StandardRef);
                Delete(item.Key);
            }
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
