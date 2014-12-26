REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.AdminSettingsTpl');
REQUIRE('chlk.activities.settings.SchoolPersonPage');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.AdminPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AdminSettingsTpl)],
        'AdminPage', EXTENDS(chlk.activities.settings.SchoolPersonPage), []);
});