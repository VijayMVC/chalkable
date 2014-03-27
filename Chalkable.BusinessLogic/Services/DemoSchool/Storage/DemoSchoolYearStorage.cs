﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolYearStorage:BaseDemoStorage<int, SchoolYear>
    {
        private Dictionary<int, SchoolYear> schoolYears = new Dictionary<int, SchoolYear>();

        public DemoSchoolYearStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(string name)
        {
            return false;
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
            schoolYears.Add(id, schoolYear);
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
            return schoolYears.First().Value;
        }


        public SchoolYear GetByDate(DateTime date)
        {
            return schoolYears.First(x => x.Value.StartDate >= date && date <= x.Value.EndDate).Value;
        }

        public void Add(IList<SchoolYear> years)
        {
            foreach (var year in years)
            {

                schoolYears.Add(year.Id, year);
            }
        }

        public IList<SchoolYear> Update(IList<SchoolYear> list)
        {
            foreach (var schoolYear in list)
            {
                var sy = GetById(schoolYear.Id);
                if (sy != null)
                {
                    schoolYears[schoolYear.Id] = schoolYear;
                }
            }
            return list;
        }

        public void Add(SchoolYear schoolYear)
        {
            schoolYears.Add(schoolYear.Id, schoolYear);
        }
    }
}
