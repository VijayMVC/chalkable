REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.school.Schools');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.school.Schools)],
        'SchoolsListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});