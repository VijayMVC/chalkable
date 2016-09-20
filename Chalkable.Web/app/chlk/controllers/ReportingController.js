REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.CustomReportTemplateService');

REQUIRE('chlk.activities.reports.StudentReportDialog');
REQUIRE('chlk.activities.reports.ReportCardsDialog');
REQUIRE('chlk.models.reports.SubmitReportCardsViewData');



NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ReportingController*/
    CLASS(
        'ReportingController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ReportingService, 'reportingService',

        [ria.mvc.Inject],
        chlk.services.GradingPeriodService, 'gradingPeriodService',

        [ria.mvc.Inject],
        chlk.services.CustomReportTemplateService, 'customReportTemplateService',

        function studentComprehensiveProgressReportAction(){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = this.gradingPeriodService.getList()
                .then(function(data){
                    var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                    //var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                    return new chlk.models.reports.SubmitStudentReportViewData(data, this.getCurrentPerson().getId(), ableDownload);
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
        },

        function reportCardsAction(){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = ria.async.wait([
                this.gradingPeriodService.getList(),
                this.customReportTemplateService.list()
            ])
            .attach(this.validateResponse_())
            .then(function(res){
                var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                return new chlk.models.reports.SubmitReportCardsViewData(res[1], res[0], reasons,ableDownload,isAbleToReadSSNumber);
            }, this);

            return this.ShadeView(chlk.activities.reports.ReportCardsDialog, res);
        },

        [[chlk.models.reports.SubmitReportCardsViewData]],
        function submitReportCardsAction(model){
            if(model.getSubmitType() == "addRecipients"){
                return this.Redirect('group', 'showGroupsFromReport', [model.getGroupIds()])
            }

            var result = this.reportingService.submitReportCards(
                model.getCustomReportTemplateId(),
                model.getTitle(),
                model.getLogo(),
                model.getRecipient(),
                model.getOrderBy(),
                model.getIdToPrint(),
                model.getStandardType(),
                this.getIdsList(model.getGroupIds(), chlk.models.id.GroupId),
                model.getGradingPeriodId(),
                this.getIdsList(model.getAttendanceReasonIds(), chlk.models.id.AttendanceReasonId),
                model.getIncludeOptions()
            )
            .attach(this.validateResponse_())
            .then(function () {
                this.BackgroundCloseView(chlk.activities.reports.ReportCardsDialog);
            }, this)
            .thenBreak();
            return this.UpdateView(chlk.activities.reports.ReportCardsDialog, result);
        },


    ])
});
