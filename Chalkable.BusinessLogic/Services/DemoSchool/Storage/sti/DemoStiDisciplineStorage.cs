using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiDisciplineStorage:BaseDemoIntStorage<DisciplineReferral>
    {
        public DemoStiDisciplineStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        }

        public IList<DisciplineReferral> GetList(int classId, DateTime date)
        {
            return data.Where(x => x.Value.SectionId == classId && x.Value.Date == date).Select(x => x.Value).ToList();
        }

        public IList<DisciplineReferral> GetList(DateTime date)
        {
            return data.Where(x => x.Value.Date == date).Select(x => x.Value).ToList();
        }

        public DisciplineReferral Create(DisciplineReferral stiDiscipline)
        {
            Add(stiDiscipline);
            return stiDiscipline;
        }

        public IList<DisciplineReferral> GetSectionDisciplineSummary(int classId, DateTime startDate, DateTime endDate)
        {
            var result = new List<DisciplineReferral>();
            for (var start = startDate; start <= endDate; start = start.AddDays(1))
            {
                result.AddRange(GetList(classId, start));
            }
            return result;
        }
    }
}
