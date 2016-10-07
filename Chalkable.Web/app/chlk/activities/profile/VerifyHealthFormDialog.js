REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.profile.SchoolPersonHealthFormsDialogTpl');

NAMESPACE('chlk.activities.profile', function(){

    /**@class chlk.activities.profile.VerifyHealthFormDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.profile.SchoolPersonHealthFormsDialogTpl)],
        'VerifyHealthFormDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

        ]);
});