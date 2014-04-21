REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.WorksheetReportTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.WorksheetReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.grading.WorksheetReportTpl)],
        'WorksheetReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});