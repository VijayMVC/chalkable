REQUIRE('chlk.templates.district.BaseStatisticTpl');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.BaseStatisticItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/DistrictSummaryGridItems.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'BaseStatisticItemsTpl', EXTENDS(chlk.templates.district.BaseStatisticTpl), [
    ])
});