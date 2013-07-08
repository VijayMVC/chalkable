REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.departments.Departments');

NAMESPACE('chlk.activities.departments', function () {

    /** @class chlk.activities.departments.DepartmentsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.departments.Departments)],
        'DepartmentsListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});