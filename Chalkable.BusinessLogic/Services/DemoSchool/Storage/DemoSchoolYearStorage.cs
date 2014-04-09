using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolYearStorage:BaseDemoStorage<int, SchoolYear>
    {
        public DemoSchoolYearStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(string name)
        {
            return data.Count(x => x.Value.Name == name) > 0;
        }

        public SchoolYear Add(int id, int schoolId, string name, string description, DateTime startDate, DateTime endDate)
        {
            var schoolYear = new SchoolYear
                {
                    Id = id,
                    Description = description,
                    Name = name,
                    StartDate = startDate,
                    EndDate = endDate,
                    SchoolRef = schoolId
                };
            data[id] = schoolYear;
            return schoolYear;
        }

        public SchoolYear Edit(int id, string name, string description, DateTime startDate, DateTime endDate)
        {
            var schoolYear = GetById(id);
            schoolYear.Name = name;
            schoolYear.Description = description;
            schoolYear.StartDate = startDate;
            schoolYear.EndDate = endDate;
            return schoolYear;
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            return data.First().Value;
        }


        public SchoolYear GetByDate(DateTime date)
        {
            return data.First(x =>  date >= x.Value.StartDate && date <= x.Value.EndDate).Value;
        }

        public void Add(IList<SchoolYear> years)
        {
            foreach (var year in years)
            {
                Add(year);
            }
        }

        public IList<SchoolYear> Update(IList<SchoolYear> list)
        {
            foreach (var schoolYear in list)
            {
                if (data.ContainsKey(schoolYear.Id))
                {
                    data[schoolYear.Id] = schoolYear;
                }
            }
            return list;
        }

        public void Add(SchoolYear schoolYear)
        {
            if (!data.ContainsKey(schoolYear.Id))
                data[schoolYear.Id] = schoolYear;
        }
    }
}
