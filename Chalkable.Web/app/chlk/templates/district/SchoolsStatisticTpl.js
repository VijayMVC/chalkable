REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.school.SchoolStatistic');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.SchoolsStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/SchoolsStatistic.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'SchoolsStatisticTpl', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.SchoolStatistic), 'items'
        ])
});