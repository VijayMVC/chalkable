REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolClassesStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolClassesSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'SchoolClassesStatisticTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [

        ])
});