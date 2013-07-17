REQUIRE('ria.mvc.TemplateActivity');
REQUIRE('chlk.templates.school.Schools');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.Schools)],
        'SchoolsListPage', EXTENDS(ria.mvc.TemplateActivity), []);
});