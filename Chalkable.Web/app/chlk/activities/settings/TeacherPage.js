REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.TeacherSettings');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.TeacherPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.TeacherSettings)],
        'TeacherPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});