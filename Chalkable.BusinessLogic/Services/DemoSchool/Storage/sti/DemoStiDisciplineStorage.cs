using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiDisciplineStorage:BaseDemoIntStorage<DisciplineReferral>
    {
        public DemoStiDisciplineStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public IList<DisciplineReferral> GetList(int classId, DateTime date)
        {
            return data.Where(x => x.Value.SectionId == classId && x.Value.Date == date).Select(x => x.Value).ToList();
        }

        public DisciplineReferral Create(DisciplineReferral stiDiscipline)
        {
            Add(stiDiscipline);
            return stiDiscipline;
        }

        public override void Setup()
        {
            Create(new DisciplineReferral
            {
                Date = DateTime.Now.Date,
                Id = GetNextFreeId(),
                Infractions = Storage.StiInfractionStorage.GetAll(),
                StudentId = 1196
            });
        }
    }
}
