using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationAccessViewData
    {
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
     
        public static ApplicationAccessViewData Create(Application application)
        {
            return new ApplicationAccessViewData
                {
                    HasAdminMyApps = application.HasAdminMyApps,
                    HasStudentMyApps = application.HasStudentMyApps,
                    HasTeacherMyApps = application.HasTeacherMyApps,
                    HasParentMyApps = application.HasParentMyApps,
                    CanAttach = application.CanAttach,
                    ShowInGradeView = application.ShowInGradeView,
                };
        }
    }
}