REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.ReportCardsSubmitFormTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.ReportCardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ReportCardsSubmitFormTpl)],
        'ReportCardsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});