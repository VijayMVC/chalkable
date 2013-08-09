REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.TeacherSettings');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.TeacherSettingsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.setup.TeacherSettings)],
        'TeacherSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});