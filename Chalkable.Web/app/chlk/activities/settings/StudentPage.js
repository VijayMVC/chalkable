REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.StudentSettingsTpl');
REQUIRE('chlk.activities.settings.SchoolPersonPage');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.StudentPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.StudentSettingsTpl)],
        'StudentPage', EXTENDS(chlk.activities.settings.SchoolPersonPage), []);
});