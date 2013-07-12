REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.templates.school.Schools');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolsGrid*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolsGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'SchoolsGrid', EXTENDS(chlk.templates.school.Schools), [])
});