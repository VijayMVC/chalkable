REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.AlertsPopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/AlertsPopUpTpl.jade')],
        [ria.templates.ModelBind(chlk.models.people.ShortUserInfo)],
        'AlertsPopUpTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'withMedicalAlert',

            [ria.templates.ModelPropertyBind],
            Boolean, 'allowedInetAccess',

            [ria.templates.ModelPropertyBind],
            String, 'specialInstructions',

            [ria.templates.ModelPropertyBind],
            String, 'spedStatus'
        ])
});