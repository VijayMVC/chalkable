REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.DeveloperSettings');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.DeveloperSettings*/

    ASSET('~/assets/jade/activities/settings/school-person-settings-page.jade')();

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/DeveloperSettings.jade')],
        [ria.templates.ModelBind(chlk.models.settings.DeveloperSettings)],
        'DeveloperSettings', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'developerId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'currentAppId',

            [ria.templates.ModelPropertyBind],
            String, 'currentAppName'
        ])
});