REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.StudentAvgPopUpTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.StudentAvgPopupDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.grading.StudentAvgPopUpTpl)],
        'StudentAvgPopupDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});