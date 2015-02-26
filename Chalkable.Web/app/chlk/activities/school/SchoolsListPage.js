REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.school.Schools');

NAMESPACE('chlk.activities.school', function () {

    /** @class chlk.activities.school.SchoolsListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.school.Schools)],
        'SchoolsListPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});