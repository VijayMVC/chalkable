REQUIRE('chlk.templates.district.BaseStatisticTpl');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolClassesStatisticItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolClassesSummaryGridItems.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'SchoolClassesStatisticItemsTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [
        ])
});