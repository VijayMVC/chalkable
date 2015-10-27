REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AppAttachModeDialogTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppAttachModeDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AppAttachModeDialogTpl)],
        'AppAttachModeDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

        ]);
});
