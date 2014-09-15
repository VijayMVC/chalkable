using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlternateScoreStorage:BaseDemoIntStorage<AlternateScore>
    {

        public DemoAlternateScoreStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }
    }
}
