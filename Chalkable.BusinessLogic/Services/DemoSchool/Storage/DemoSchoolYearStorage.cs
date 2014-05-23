using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolYearStorage:BaseDemoIntStorage<SchoolYear>
    {
        public DemoSchoolYearStorage(DemoStorage storage) : base(storage, x => x.Id)
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

        public override void Setup()
        {
            var currentDate = DateTime.Now;
            Add(DemoSchoolConstants.CurrentSchoolYearId, DemoSchoolConstants.SchoolId, "Current School Year", "", 
                new DateTime(currentDate.Year, 1, 1),
                new DateTime(currentDate.Year, 12, 31));
        }
    }
}
