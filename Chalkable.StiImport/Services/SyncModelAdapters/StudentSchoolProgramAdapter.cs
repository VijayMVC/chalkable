using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentSchoolProgramAdapter : SyncModelAdapter<StudentSchoolProgram>
    {
        public StudentSchoolProgramAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StudentSchoolProgram Selector(StudentSchoolProgram x)
        {
            return new Data.School.Model.StudentSchoolProgram
            {
                Id = x.StudentSchoolProgramID,
                AcadSessionId = x.AcadSessionID,
                DecimalValue = x.DecimalValue,
                EndDate = x.EndDate,
                EndTime = x.EndTime,
                SchoolProgramId = x.SchoolProgramID,
                StartDate = x.StartDate,
                StudentId = x.StudentID
            };
        }

        protected override void InsertInternal(IList<StudentSchoolProgram> entities)
        {
            ServiceLocatorSchool.StudentSchoolProgramService.Add(entities.Select(Selector).ToList());
        }

        protected override void UpdateInternal(IList<StudentSchoolProgram> entities)
        {
            ServiceLocatorSchool.StudentSchoolProgramService.Edit(entities.Select(Selector).ToList());
        }

        protected override void DeleteInternal(IList<StudentSchoolProgram> entities)
        {
            ServiceLocatorSchool.StudentSchoolProgramService.Delete(entities.Select(Selector).ToList());
        }

        protected override void PrepareToDeleteInternal(IList<StudentSchoolProgram> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
