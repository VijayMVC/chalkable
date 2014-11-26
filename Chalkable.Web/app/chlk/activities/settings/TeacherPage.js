REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.TeacherSettingsTpl');
REQUIRE('chlk.activities.settings.SchoolPersonPage');
REQUIRE('chlk.templates.settings.ChangePasswordTpl');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.TeacherPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.TeacherSettingsTpl)],
        'TeacherPage', EXTENDS(chlk.activities.settings.SchoolPersonPage), [

        ]);
});