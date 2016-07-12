REQUIRE('chlk.templates.classes.LECreditsDialogTpl');

NAMESPACE('chlk.activities.classes', function(){

    /**@class chlk.activities.classes.LECreditsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.classes.LECreditsDialogTpl)],

        'LECreditsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});