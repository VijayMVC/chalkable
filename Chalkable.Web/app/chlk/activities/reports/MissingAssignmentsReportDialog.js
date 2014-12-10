REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.MissingAssignmentsReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.MissingAssignmentsReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.MissingAssignmentsReportTpl)],
        'MissingAssignmentsReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

        ]);
});