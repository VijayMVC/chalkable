REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.settings.DeveloperSettings');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.DeveloperSettings*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/DeveloperSettings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.DeveloperSettings)],
        'DeveloperSettings', EXTENDS(chlk.templates.JadeTemplate), [
        ])
});