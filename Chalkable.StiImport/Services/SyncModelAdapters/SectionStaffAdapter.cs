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

        private ClassTeacher Selector(SectionStaff x)
        {
            return new ClassTeacher
            {
                ClassRef = x.SectionID,
                IsCertified = x.IsCertified,
                IsHighlyQualified = x.IsHighlyQualified,
                IsPrimary = x.IsPrimary,
                PersonRef = x.StaffID
            };
        }

        protected override void InsertInternal(IList<SectionStaff> entities)
        {
            var teachers = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.AddTeachers(teachers);
        }

        protected override void UpdateInternal(IList<SectionStaff> entities)
        {
            var teachers = entities.Select(Selector).ToList();
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

        protected override void PrepareToDeleteInternal(IList<SectionStaff> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}