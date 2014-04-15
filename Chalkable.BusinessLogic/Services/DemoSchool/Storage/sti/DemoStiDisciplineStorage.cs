using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiDisciplineStorage:BaseDemoStorage<int, DisciplineReferral>
    {
        public DemoStiDisciplineStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<DisciplineReferral> GetList(int classId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public DisciplineReferral Create(DisciplineReferral stiDiscipline)
        {
            throw new NotImplementedException();
        }

        public void Update(DisciplineReferral stiDiscipline)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
