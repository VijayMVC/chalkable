using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoSchoolYearStorage
    {
        private Dictionary<int, SchoolYear> schoolYears = new Dictionary<int, SchoolYear>(); 
        public DemoSchoolYearStorage()
        {
            var currentDate = DateTime.Now;
            schoolYears.Add(1, new SchoolYear
            {

                StartDate = new DateTime(currentDate.Year, 1, 1),
                EndDate = new DateTime(currentDate.Year, 12, 31),
                Id = 1,
                Description = "Current School Year",
                Name = "Current"
            });
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

        public SchoolYear GetById(int id)
        {
            return schoolYears.First(x => x.Key == id).Value;
        }

        public SchoolYear GetCurrentSchoolYear()
        {
            return schoolYears.First().Value;
        }

        public void Delete(int schoolYearId)
        {
            schoolYears.Remove(schoolYearId);
        }

        public IList<SchoolYear> GetAll()
        {
            return schoolYears.Select(x => x.Value).ToList();
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

        public void Delete(IList<int> schoolYearIds)
        {
            foreach (var schoolYearId in schoolYearIds)
            {
                Delete(schoolYearId);
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
