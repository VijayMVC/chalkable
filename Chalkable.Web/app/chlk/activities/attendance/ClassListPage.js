REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.ClassList');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.ClassListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.ClassList)],
        'ClassListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});