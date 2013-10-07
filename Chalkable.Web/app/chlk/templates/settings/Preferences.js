REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.settings.PreferencesList');


NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.Preferences*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/Preferences.jade')],
        [ria.templates.ModelBind(chlk.models.settings.PreferencesList)],
        'Preferences', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.settings.Preference), 'items'
        ])
});