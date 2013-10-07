REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.settings.Preference');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.Start*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/Start.jade')],
        [ria.templates.ModelBind(chlk.models.settings.Preference)],
        'Start', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'type'
        ])
});