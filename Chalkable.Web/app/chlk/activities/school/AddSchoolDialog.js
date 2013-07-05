REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.AddSchool.Dialog');

NAMESPACE('chlk.activities.school', function () {
     /** @class chlk.activities.AddSchoolDialog */
     CLASS(
        [ria.mvc.ActivityGroup('AddSchool')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.lib.BindTemplate(chlk.templates.AddSchool.Dialog)],
        'AddSchoolDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
