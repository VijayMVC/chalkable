REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.school.AddSchoolDialog');

NAMESPACE('chlk.activities.school', function () {
     /** @class chlk.activities.school.AddSchoolDialog */
     CLASS(
        [ria.mvc.ActivityGroup('AddSchool')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.AddSchoolDialog)],
        'AddSchoolDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
