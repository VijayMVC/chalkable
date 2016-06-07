using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationAccessViewData
    {
        public bool HasStudentMyApps { get; set; }
        public bool HasTeacherMyApps { get; set; }
        public bool HasAdminMyApps { get; set; }
        public bool HasParentMyApps { get; set; }
        public bool HasTeacherExternalAttach { get; set; }
        public bool HasStudentExternalAttach { get; set; }
        public bool HasAdminExternalAttach { get; set; }
        public bool HasSysAdminSettings { get; set; }
        public bool HasDistrictAdminSettings { get; set; }
        public bool HasStudentProfile { get; set; }
        public bool CanAttach { get; set; }
        public bool ShowInGradeView { get; set; }
        public bool ProvidesRecommendedContent { get; set; }

        public static ApplicationAccessViewData Create(Application application)
        {
            return new ApplicationAccessViewData
                {
                    HasAdminMyApps = application.HasAdminMyApps,
                    HasStudentMyApps = application.HasStudentMyApps,
                    HasTeacherMyApps = application.HasTeacherMyApps,
                    HasParentMyApps = application.HasParentMyApps,
                    HasAdminExternalAttach = application.HasAdminExternalAttach,
                    HasTeacherExternalAttach = application.HasTeacherExternalAttach,
                    HasStudentExternalAttach = application.HasStudentExternalAttach,
                    HasSysAdminSettings = application.HasSysAdminSettings,
                    HasDistrictAdminSettings = application.HasDistrictAdminSettings,
                    HasStudentProfile = application.HasStudentProfile,
                    CanAttach = application.CanAttach,
                    ShowInGradeView = application.ShowInGradeView,
                    ProvidesRecommendedContent = application.ProvidesRecommendedContent
                };
        }
    }
}