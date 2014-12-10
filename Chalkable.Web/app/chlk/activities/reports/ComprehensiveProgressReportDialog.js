REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.ComprehensiveProgressReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.ComprehensiveProgressReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ComprehensiveProgressReportTpl)],
        'ComprehensiveProgressReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

    ]);
});