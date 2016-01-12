REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolClassesStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolClassesSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'SchoolClassesStatisticTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [

        ])
});