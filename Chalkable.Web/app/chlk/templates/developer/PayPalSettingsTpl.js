REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.developer.PayPalInfo');

NAMESPACE('chlk.templates.developer', function () {

    /** @class chlk.templates.developer.PayPalSettingsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/developer/paypalSettings.jade')],
        [ria.templates.ModelBind(chlk.models.developer.PayPalInfo)],
        'PayPalSettingsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'email'
        ])
});