using System.Collections.Generic;
using System.Linq;
using StudentCustomAlertDetail = Chalkable.StiConnector.SyncModel.StudentCustomAlertDetail;


namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StudentCustomAlertDetailAdapter : SyncModelAdapter<StudentCustomAlertDetail>
    {
        public StudentCustomAlertDetailAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StudentCustomAlertDetail Selector(StudentCustomAlertDetail x)
        {
            return new Data.School.Model.StudentCustomAlertDetail
            {
                Id = x.StudentCustomAlertDetailID,
                CustomAlertDetailId = x.CustomAlertDetailID,
                StudentRef = x.StudentID,
                SchoolYearRef = x.AcadSessionID,
                AlertText = x.AlertText,
                CurrentValue = x.CurrentValue
            };
        }

        protected override void InsertInternal(IList<StudentCustomAlertDetail> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentCustomAlertDetailService.Add(sts);
        }

        protected override void UpdateInternal(IList<StudentCustomAlertDetail> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StudentCustomAlertDetailService.Edit(sts);
        }
        protected override void DeleteInternal(IList<StudentCustomAlertDetail> entities)
        {
            var toDelete = entities.Select(x => new Data.School.Model.StudentCustomAlertDetail
            {
                Id = x.StudentCustomAlertDetailID
            }).ToList();
            ServiceLocatorSchool.StudentCustomAlertDetailService.Delete(toDelete);
        }
    }
}
