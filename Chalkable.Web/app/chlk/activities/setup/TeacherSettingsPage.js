REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.Final');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.TeacherSettingsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.Final)],
        'TeacherSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

        ]);
});