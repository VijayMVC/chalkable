REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.departments.DepartmentDialog');

NAMESPACE('chlk.activities.departments', function () {
     /** @class chlk.activities.departments.DepartmentDialog */
     CLASS(
        [ria.mvc.ActivityGroup('DepartmentDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.departments.DepartmentDialog)],
        'DepartmentDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
