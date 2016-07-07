REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.DeveloperProfile');

REQUIRE('chlk.templates.settings.ChangePasswordTpl');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.DeveloperPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.DeveloperProfile)],
        'DeveloperPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '#changePasswordLink,#cancell-edit-pwd-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function onChangePwdClick($node, event) {
                this.dom.find('#changePasswordLink').toggleClass('x-hidden');
                this.dom.find('#changePasswordForm').toggleClass('x-hidden');
            }
        ]);
});