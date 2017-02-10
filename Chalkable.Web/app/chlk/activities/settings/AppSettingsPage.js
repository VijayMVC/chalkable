REQUIRE('chlk.activities.apps.AppWrapperPage');
REQUIRE('chlk.templates.settings.AppSettingsPageTpl');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.AppSettingsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AppSettingsPageTpl)],
        'AppSettingsPage', EXTENDS(chlk.activities.apps.AppWrapperPage), [

        ]);
});
