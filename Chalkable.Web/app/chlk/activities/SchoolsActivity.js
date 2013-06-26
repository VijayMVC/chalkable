REQUIRE('chlk.activities.TemplateActivity');
REQUIRE('chlk.templates.Schools');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.SchoolsActivity */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.BindTemplate(chlk.templates.Schools)],
        'SchoolsActivity', EXTENDS(chlk.activities.TemplateActivity), [ ]);
});