REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.CategoryDialog');
 
 NAMESPACE('chlk.activities.apps', function () {
     /** @class chlk.activities.apps.CategoryDialog*/
      CLASS(
        [ria.mvc.ActivityGroup('CategoryDialog')],
         [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.CategoryDialog)],
        'CategoryDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
});
