using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentProfileViewData : StudentViewData
    {
        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }
        public IList<StudentCustomAlertDetailViewData> StudentCustomAlertDetails { get; set; }
        public bool HasNotVerifiedHealthForm { get; set; }

        protected StudentProfileViewData(Student student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions, IList<StudentHealthFormInfo> studentHealthForms) : base(student)
        {
            if(healthConditions != null)
                HealthConditions = StudentHealthConditionViewData.Create(healthConditions);
            if(customAlerts != null)
                StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts);
            HasNotVerifiedHealthForm = studentHealthForms.Count > 0 && studentHealthForms.Any(x => !x.VerifiedDate.HasValue);
        }

        public static StudentProfileViewData Create(Student student, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions, IList<StudentHealthFormInfo> studentHealthForms)
        {
            return new StudentProfileViewData(student, customAlerts, healthConditions, studentHealthForms);
        }
    }
}