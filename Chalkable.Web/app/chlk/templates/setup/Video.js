REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.settings.Preference');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.Video*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/Video.jade')],
        [ria.templates.ModelBind(chlk.models.settings.Preference)],
        'Video', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'value',

            [ria.templates.ModelPropertyBind],
            Number, 'type'
        ])
});