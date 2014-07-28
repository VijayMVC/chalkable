REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.ChangePasswordTpl');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.ChangePasswordPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.ChangePasswordTpl)],
        'ChangePasswordPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});