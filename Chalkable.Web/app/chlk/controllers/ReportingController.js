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
                    return new chlk.models.reports.SubmitStudentReportViewData(data, this.getCurrentPerson().getId(), ableDownload);
                }, this);

            return this.ShadeView(chlk.activities.reports.StudentReportDialog, res);
        },

        [[chlk.models.reports.SubmitStudentReportViewData]],
        function submitStudentComprehensiveProgressReportAction(reportViewData){
            var src = this.reportingService.submitStudentComprehensiveProgressReport(
                reportViewData.getGradingPeriodId(),
                this.getCurrentPerson().getId()
            );
            this.BackgroundCloseView(chlk.activities.reports.StudentReportDialog);
            this.getContext().getDefaultView().submitToIFrame(src);
            return null;
        }

    ])
});
