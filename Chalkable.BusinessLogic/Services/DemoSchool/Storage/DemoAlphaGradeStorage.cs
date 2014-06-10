using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlphaGradeStorage:BaseDemoIntStorage<AlphaGrade>
    {
        public DemoAlphaGradeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<AlphaGrade> GetForClassStandarts(int classId)
        {
            return GetAll();
        }

        public IList<AlphaGrade> GetForClass(int classId)
        {
            return GetAll();
        }
    }
}
