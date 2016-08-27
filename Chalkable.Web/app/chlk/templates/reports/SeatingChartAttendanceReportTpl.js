REQUIRE('chlk.templates.reports.BaseAttendanceReportTpl');
REQUIRE('chlk.models.reports.BaseReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.SeatingChartAttendanceReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/SeatingChartReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.BaseReportViewData)],
        'SeatingChartAttendanceReportTpl', EXTENDS(chlk.templates.reports.BaseAttendanceReportTpl), [])
});