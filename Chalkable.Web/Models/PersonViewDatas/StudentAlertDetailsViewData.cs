using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentAlertDetailsViewData
    {
        public bool HasMedicalAlert { get; set; }
        public bool IsAllowedInetAccess { get; set; }
        public string SpecialInstructions { get; set; }
        public string SpEdStatus { get; set; }
        public bool IsIEPActive { get; set; }

        public IList<StudentHealthConditionViewData> HealthConditions { get; set; }
        public IList<StudentCustomAlertDetailViewData> StudentCustomAlertDetails { get; set; }

        public static StudentAlertDetailsViewData Create(Student student, IList<StudentHealthCondition> healthConditions,
            IList<StudentCustomAlertDetail> customAlerts)
        {
            return new StudentAlertDetailsViewData
            {
                HealthConditions = StudentHealthConditionViewData.Create(healthConditions),
                StudentCustomAlertDetails = StudentCustomAlertDetailViewData.Create(customAlerts),
                IsAllowedInetAccess = student.IsAllowedInetAccess,
                SpecialInstructions = student.SpecialInstructions,
                HasMedicalAlert = student.HasMedicalAlert,
                SpEdStatus = student.SpEdStatus,
                IsIEPActive = student.IsIEPActive
            };
        }
    }
}