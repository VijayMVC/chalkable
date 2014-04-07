﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentSchoolYearStorage:BaseDemoStorage<int, StudentSchoolYear>
    {
        private int index = 0;
        public DemoStudentSchoolYearStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(StudentSchoolYear studentSchoolYear)
        {

            data.Add(index++, studentSchoolYear);
        }

        public void Add(IList<StudentSchoolYear> studentSchoolYears)
        {
            foreach (var schoolYear in studentSchoolYears)
            {
                Add(schoolYear);
            }
        }

        public IList<StudentSchoolYear> GetAll(int personId)
        {
            return data.Where(x => x.Value.StudentRef == personId).Select(x => x.Value).ToList();
        } 
    }
}
