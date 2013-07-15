REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.departments.AddDepartmentDialog');

NAMESPACE('chlk.activities.departments', function () {
     /** @class chlk.activities.departments.AddDepartmentDialog */
     CLASS(
        [ria.mvc.ActivityGroup('AddDepartment')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.departments.AddDepartmentDialog)],
        'AddDepartmentDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
