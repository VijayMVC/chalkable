REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.SeatingChartAttendanceReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.SeatingChartAttendanceReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.SeatingChartAttendanceReportTpl)],
        'SeatingChartAttendanceReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});