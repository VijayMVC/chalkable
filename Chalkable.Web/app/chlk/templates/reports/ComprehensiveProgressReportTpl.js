REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.SubmitComprehensiveProgressViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.ComprehensiveProgressReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/ComprehensiveProgressReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.SubmitComprehensiveProgressViewData)],
        'ComprehensiveProgressReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons'
        ]);
});