REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AddCategoryDialog');

NAMESPACE('chlk.activities.apps', function () {
     /** @class chlk.activities.apps.AddCategoryDialog*/
     CLASS(
        [ria.mvc.ActivityGroup('AddDepartment')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddCategoryDialog)],
        'AddCategoryDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
