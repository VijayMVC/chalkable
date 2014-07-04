REQUIRE('chlk.activities.settings.ChangePasswordPage');
REQUIRE('chlk.templates.settings.SchoolPersonSettingsTpl');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.SchoolPersonPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.SchoolPersonSettingsTpl)],
        'SchoolPersonPage', EXTENDS(chlk.activities.settings.ChangePasswordPage), [
        ]);
});