REQUIRE('chlk.templates.reports.BaseReportTpl');
REQUIRE('chlk.models.reports.BaseReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.SeatingChartReportTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/SeatingChartReport.jade')],
        [ria.templates.ModelBind(chlk.models.reports.BaseReportViewData)],
        'SeatingChartReportTpl', EXTENDS(chlk.templates.reports.BaseReportTpl), [])
});