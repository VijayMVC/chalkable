REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.SeatingChartReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.SeatingChartReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.SeatingChartReportTpl)],
        'SeatingChartReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});