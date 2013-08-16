REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.ChangePassword');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.ChangePasswordPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.ChangePassword)],
        'ChangePasswordPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});
