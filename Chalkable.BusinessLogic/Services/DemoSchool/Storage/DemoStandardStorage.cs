using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStandardStorage:BaseDemoStorage<int ,Standard>
    {
        public DemoStandardStorage(DemoStorage storage) : base(storage)
        {
        }

        public void AddStandards(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (!data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }

        public IList<Standard> GetStandarts(StandardQuery query)
        {
            var standards = data.Select(x => x.Value);
            if (query.StandardSubjectId.HasValue)
                standards = standards.Where(x => x.StandardSubjectRef == query.StandardSubjectId);
            if (query.GradeLavelId.HasValue)
                standards =
                    standards.Where(
                        x => query.GradeLavelId <= x.LowerGradeLevelRef && query.GradeLavelId >= x.UpperGradeLevelRef);
            if (!query.AllStandards || query.ParentStandardId.HasValue)
                standards = standards.Where(x => x.ParentStandardRef == query.ParentStandardId);

            if (query.ClassId.HasValue)
            {
                var classStandarts = Storage.ClasStandardStorage.GetAll(query.ClassId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }
            if (query.CourseId.HasValue)
            {
                var classStandarts = Storage.ClasStandardStorage.GetAll(query.CourseId).Select(x => x.StandardRef);
                standards = standards.Where(x => classStandarts.Contains(x.Id));
            }

            return standards.ToList();
        }

        public void Update(IList<Standard> standards)
        {
            foreach (var standard in standards)
            {
                if (data.ContainsKey(standard.Id))
                    data[standard.Id] = standard;
            }
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
