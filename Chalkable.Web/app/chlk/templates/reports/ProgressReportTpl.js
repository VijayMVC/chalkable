REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.templates.grading.GradingCommentsDropdownTpl');
REQUIRE('chlk.models.reports.SubmitProgressReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.ProgressReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/ProgressReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitProgressReportViewData)],
        'ProgressReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.reports.UserForReport), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',
        ])
});