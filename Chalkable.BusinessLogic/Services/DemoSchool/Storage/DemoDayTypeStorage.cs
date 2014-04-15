using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoDayTypeStorage : BaseDemoStorage<int, DayType>
    {
        public DemoDayTypeStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<DayType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var dayTypes = data.Select(x => x.Value).Where(x => x.SchoolYearRef == schoolYearId);

            if (fromNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number >= fromNumber);
            if (toNumber.HasValue)
                dayTypes = dayTypes.Where(x => x.Number <= toNumber);

            return dayTypes.ToList();
        }


        public void Add(DayType ss)
        {
            if (!data.ContainsKey(ss.Id))
                data[ss.Id] = ss;
        }

        public IList<DayType> Update(IList<DayType> dayTypes)
        {
            foreach (var dayType in dayTypes)
            {
                if (data.ContainsKey(dayType.Id))
                    data[dayType.Id] = dayType;
            }
            return dayTypes;
        }


        public bool Exists(int schoolYearId)
        {
            return data.Count(x => x.Value.SchoolYearRef == schoolYearId) > 0;
        }

        public IList<DayType> Add(IList<DayType> dayTypes)
        {
            foreach (var dayType in dayTypes)
            {
                Add(dayType);
            }
            return dayTypes;
        }

        public void Delete(DayType dayType)
        {
            data.Remove(dayType.Id);
        }

        public override void Setup()
        {
            Add(new DayType
            {
                Id = 1,
                Name = "M",
                Number = 0,
                SchoolYearRef = 12
            });

            Add(new DayType
            {
                Id = 2,
                Name = "TTh",
                Number = 1,
                SchoolYearRef = 12
            });

            Add(new DayType
            {
                Id = 3,
                Name = "WF",
                Number = 2,
                SchoolYearRef = 12
            });
        }
    }
}
