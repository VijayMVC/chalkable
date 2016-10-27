using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.Reporting;
using Chalkable.BusinessLogic.Services.Reporting.ReportingGenerators;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model.Reports;
using Chalkable.StiConnector.Connectors.Model.Reports.ReportCards;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IReportingService
    {
        IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId);
        void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment);
        void SetProgressReportComments(int classId, int gradingPeriodId, IList<StudentCommentInputModel> studentComments);

        byte[] GenerateReport<TReportSettings>(TReportSettings settings) where TReportSettings : class;
        
        byte[] DownloadReport(string reportId);
        void ScheduleReportCardTask(ReportCardsInputModel inputModel);
        void GenerateReportCard(ReportCardsInputModel inputModel);
        FeedReportSettingsInfo GetFeedReportSettings();
        void SetFeedReportSettings(FeedReportSettingsInfo feedReportSettings);

        IList<ReportCardsLogo> GetReportCardsLogos();
        ReportCardsLogo GetLogoBySchoolId(int schoolId);
        ReportCardsLogo GetDistrictLogo();
        void UpdateReportCardsLogo(int? schoolId, byte[] logoIcon);
        void DeleteReportCardsLogo(int id);
    }

    

    public class ReportingService : SisConnectedService, IReportingService
    {
        public ReportingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId)
        {
            var inowReportComments = ConnectorLocator.ReportConnector.GetProgressReportComments(classId, gradingPeriodId);
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            int markingPeriodId = gp.MarkingPeriodRef;
            var students = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriodId);
            var res = new List<StudentCommentInfo>();
            foreach (var student in students)
            {
                var studentComment = inowReportComments.FirstOrDefault(sc => sc.StudentId == student.Id);
                if (studentComment == null) continue;
                res.Add(new StudentCommentInfo
                {
                    Student = student,
                    Comment = studentComment.Comment
                });
            }
            return res;
        }

        public void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment)
        {
            var stComment = new StudentCommentInputModel {Comment = comment, StudentId = studentId};
            SetProgressReportComments(classId, gradingPeriodId, new List<StudentCommentInputModel> {stComment});
        }
        
        public void SetProgressReportComments(int classId, int gradingPeriodId, IList<StudentCommentInputModel> studentComments)
        {
            if (studentComments == null || studentComments.Count <= 0) return;
            var inowStudentProgressReportComments =
                studentComments.Select(x => new StudentProgressReportComment
                    {
                        Comment = x.Comment,
                        StudentId = x.StudentId,
                        GradingPeriodId = gradingPeriodId,
                        SectionId = classId
                    }).ToList();
            ConnectorLocator.ReportConnector.UpdateProgressReportComment(classId, inowStudentProgressReportComments);
        }

        public byte[] GenerateReport<TReportSettings>(TReportSettings settings) where TReportSettings : class
        {
            return ReportGeneratorFactory.CreateGenerator<TReportSettings>(ServiceLocator, ConnectorLocator).GenerateReport(settings);
        }
        public void ScheduleReportCardTask(ReportCardsInputModel inputModel)
        {
            Trace.Assert(Context.DistrictId.HasValue);

            if (!ServiceLocator.ServiceLocatorMaster.DistrictService.IsReportCardsEnabled())
                throw new ChalkableSecurityException("This district hasn't access Report Cards!");

            var data = new ReportProcessingTaskData(Context, inputModel, inputModel.ContentUrl);
            var scheduleTime = DateTime.UtcNow.AddDays(-20);

            ServiceLocator.ServiceLocatorMaster.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.GenerateReport, scheduleTime,
                Context.DistrictId, data.ToString(), $"{Context.DistrictId.Value}_report_processing");

        }

        public void GenerateReportCard(ReportCardsInputModel inputModel)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.DistrictId.HasValue);
            try
            {
                var content = ServiceLocator.ReportService.GenerateReport(inputModel);
                var reportId = $"reportcard_{Context.DistrictId.Value}_{Context.PersonId.Value}_{Guid.NewGuid()}";
                ServiceLocator.StorageBlobService.AddBlob("reports", reportId, content);
                ServiceLocator.NotificationService.AddReportProcessedNotification(Context.PersonId.Value, Context.RoleId, "Report Cards", reportId, null, true);
            }
            catch (Exception e)
            {
                ServiceLocator.NotificationService.AddReportProcessedNotification(Context.PersonId.Value, Context.RoleId, "Report Cards", null, "We had an issue building your report. Please try again.", false);
                throw e;
            }
        }

        public byte[] DownloadReport(string reportId)
        {
            return ServiceLocator.StorageBlobService.GetBlobContent("reports", reportId);
        }



        public FeedReportSettingsInfo GetFeedReportSettings()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var settings = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value,
                Context.SchoolYearId.Value, new List<string>
                {
                    PersonSetting.FEED_REPORT_START_DATE,
                    PersonSetting.FEED_REPORT_END_DATE,
                    PersonSetting.FEED_REPORT_INCLUDE_DETAILS,
                    PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ACTIVITIES,
                    PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ATTRIBUTES,
                    PersonSetting.FEED_REPORT_LP_ONLY,
                    PersonSetting.FEED_REPORT_INCLUDE_ATTACHMENTS
                });
            return new FeedReportSettingsInfo(settings);
        }

        public void SetFeedReportSettings(FeedReportSettingsInfo settings)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            ValidateDateRange(settings.StartDate, settings.EndDate);
            ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value, Context.SchoolYearId.Value, settings.ToDictionary());
        }

        public IList<ReportCardsLogo> GetReportCardsLogos()
        {
            var res = DoRead(u => new DataAccessBase<ReportCardsLogo>(u).GetAll());
            return res;
        }

        public ReportCardsLogo GetLogoBySchoolId(int schoolId)
        {
            var conds = new AndQueryCondition {{nameof(ReportCardsLogo.SchoolRef), schoolId}};
            return DoRead(u => new DataAccessBase<ReportCardsLogo>(u).GetAll(conds)).FirstOrDefault();
        }

        public ReportCardsLogo GetDistrictLogo()
        {
            var conds = new AndQueryCondition {{nameof(ReportCardsLogo.SchoolRef), null}};
            return DoRead(u => new DataAccessBase<ReportCardsLogo>(u).GetAll(conds)).FirstOrDefault();
        }

        public void UpdateReportCardsLogo(int? schoolId, byte[] logoIcon)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u =>
            {
                var da = new DataAccessBase<ReportCardsLogo, int>(u);
                var res = da.GetAll(new AndQueryCondition {{nameof(ReportCardsLogo.SchoolRef), schoolId}})
                            .FirstOrDefault();
                var logoAddress = UploadLogo(schoolId, logoIcon);
                if (res == null)
                {
                    res = new ReportCardsLogo { SchoolRef = schoolId , LogoAddress = logoAddress};
                    da.Insert(res);
                }
                else if (logoIcon == null)
                {
                    da.Delete(res.Id);
                }
                else
                {
                    res.SchoolRef = schoolId;
                    res.LogoAddress = logoAddress;
                    da.Update(res);
                }
            });
        }

        private string UploadLogo(int? schoolId, byte[] logo)
        {
            Trace.Assert(Context.DistrictId.HasValue);
            var key = $"reportcardslogo_{Context.DistrictId.Value}";
            if (schoolId.HasValue) key += $"_{schoolId.Value}";
            ServiceLocator.StorageBlobService.AddBlob("pictureconteiner", key, logo);
            return (new BlobHelper()).GetBlobsRelativeAddress("pictureconteiner", key);
        }

        public void DeleteReportCardsLogo(int id)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new DataAccessBase<ReportCardsLogo, int>(u).Delete(id));
        }

        private static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ChalkableException("Invalid date range. Start date is greater than end date");
        }
    }

}
