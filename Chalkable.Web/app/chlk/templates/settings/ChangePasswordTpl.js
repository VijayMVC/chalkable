REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.account.ChangePassword');

NAMESPACE('chlk.templates.settings', function () {

    /** @class chlk.templates.settings.ChangePasswordTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/settings/ChangePasswordView.jade')],
        [ria.templates.ModelBind(chlk.models.account.ChangePassword)],
        'ChangePasswordTpl', EXTENDS(chlk.templates.ChlkTemplate), [

        ])
});