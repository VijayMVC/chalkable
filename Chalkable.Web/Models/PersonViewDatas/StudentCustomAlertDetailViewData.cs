using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Microsoft.ReportingServices.Interfaces;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentCustomAlertDetailViewData
    {
        public int Id { get; set; }
        public int CustomAlertDetailId { get; set; }
        public int StudentRef { get; set; }
        public int SchoolYearRef { get; set; }
        public string AlertText { get; set; }
        public string CurrentValue { get; set; }
        public static StudentCustomAlertDetailViewData Create(StudentCustomAlertDetail customAlert)
        {
            return new StudentCustomAlertDetailViewData
            {
                Id = customAlert.Id,
                AlertText = customAlert.AlertText,
                CurrentValue = customAlert.CurrentValue,
                CustomAlertDetailId = customAlert.CustomAlertDetailId,
                SchoolYearRef = customAlert.SchoolYearRef,
                StudentRef = customAlert.StudentRef
            };
        }

        public static IList<StudentCustomAlertDetailViewData> Create(IList<StudentCustomAlertDetail> customeAlerts)
        {
            return customeAlerts.Select(Create).ToList();
        } 
    }
}