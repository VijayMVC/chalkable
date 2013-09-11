REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.InstallAppDialogTpl');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.InstallAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.InstallAppDialogTpl)],
        'InstallAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
});