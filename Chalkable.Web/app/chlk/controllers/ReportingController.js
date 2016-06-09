REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.GradingPeriodService');

REQUIRE('chlk.activities.reports.StudentReportDialog');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ReportingController*/
    CLASS(
        'ReportingController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ReportingService, 'reportingService',

        [ria.mvc.Inject],
        chlk.services.GradingPeriodService, 'gradingPeriodService',

        function studentComprehensiveProgressReportAction(){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = this.gradingPeriodService.getList()
                .then(function(data){
                    var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                    var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                    return new chlk.models.reports.SubmitStudentReportViewData(data, this.getCurrentPerson().getId(), ableDownload, isAbleToReadSSNumber);
                }, this);

            return this.ShadeView(chlk.activities.reports.StudentReportDialog, res);
        },

        [[chlk.models.reports.SubmitStudentReportViewData]],
        function submitStudentComprehensiveProgressReportAction(reportViewData){
            var result = this.reportingService.submitStudentComprehensiveProgressReport(
                    reportViewData.getGradingPeriodId(),
                    this.getCurrentPerson().getId()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.reports.StudentReportDialog);
                }, this)
                .thenBreak();

            return this.UpdateView(chlk.activities.reports.StudentReportDialog, result);
        }

    ])
});
