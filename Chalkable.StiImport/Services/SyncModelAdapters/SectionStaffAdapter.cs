using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SectionStaffAdapter : SyncModelAdapter<SectionStaff>
    {
        public SectionStaffAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<SectionStaff> entities)
        {
            var teachers = entities.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.AddTeachers(teachers);
        }

        protected override void UpdateInternal(IList<SectionStaff> entities)
        {
            var teachers = entities.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.EditTeachers(teachers);
        }

        protected override void DeleteInternal(IList<SectionStaff> entities)
        {
            var teachers = entities.Select(x => new ClassTeacher
            {
                ClassRef = x.SectionID,
                PersonRef = x.StaffID
            }).ToList();
            ServiceLocatorSchool.ClassService.DeleteTeachers(teachers);
        }
    }
}