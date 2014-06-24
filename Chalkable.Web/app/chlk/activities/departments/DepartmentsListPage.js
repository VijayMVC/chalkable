REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.departments.Departments');

NAMESPACE('chlk.activities.departments', function () {

    /** @class chlk.activities.departments.DepartmentsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.departments.Departments)],
        'DepartmentsListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});