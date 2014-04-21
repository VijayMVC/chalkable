REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.GradeBookReportTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.GradeBookReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradeBookReportTpl)],
        'GradeBookReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});