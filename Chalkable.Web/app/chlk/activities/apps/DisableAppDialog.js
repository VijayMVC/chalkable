REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.DisableAppDialogTpl');

NAMESPACE('chlk.activities.apps', function(){

    /**@class chlk.activities.apps.DisableAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.DisableAppDialogTpl)],

        'DisableAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[]);
});