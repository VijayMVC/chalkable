REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.StudentReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.StudentReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.StudentReportTpl)],
        'StudentReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});