REQUIRE('chlk.templates.apps.AppWrapperPageTpl');
REQUIRE('chlk.models.settings.AppSettingsViewData');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.AppSettingsPageTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/app-settings-page.jade')],
        [ria.templates.ModelBind(chlk.models.settings.AppSettingsViewData)],
        'AppSettingsPageTpl', EXTENDS(chlk.templates.apps.AppWrapperPageTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.apps.Application), 'applications'
        ]);
});
