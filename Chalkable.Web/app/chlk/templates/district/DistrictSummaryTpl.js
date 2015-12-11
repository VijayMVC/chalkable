REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.district.DistrictFullSummaryViewData');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.DistrictSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/DistrictSummary.jade')],
        [ria.templates.ModelBind(chlk.models.district.DistrictFullSummaryViewData)],
        'DistrictSummaryTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'schoolsStatistic',

            [ria.templates.ModelPropertyBind],
            chlk.models.district.DistrictShortSummaryViewData, 'shortSummary'
        ])
});