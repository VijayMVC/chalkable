REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/Schools.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'Schools', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.School), 'items'
        ])
});