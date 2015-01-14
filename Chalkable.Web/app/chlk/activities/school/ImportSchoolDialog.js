REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.school.ImportSchoolDialog');

NAMESPACE('chlk.activities.school', function () {
     /** @class chlk.activities.school.ImportSchoolDialog */
     CLASS(
        [ria.mvc.ActivityGroup('AddSchool')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.school.ImportSchoolDialog)],
        'ImportSchoolDialog', EXTENDS(chlk.activities.lib.TemplateDialog), []);
 });
