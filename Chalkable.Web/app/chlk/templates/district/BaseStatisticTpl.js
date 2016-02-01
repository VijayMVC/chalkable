REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.BaseStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/DistrictSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.admin.BaseStatisticGridViewData)],
        'BaseStatisticTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.admin.BaseStatistic), 'items',

            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            Number, 'start',

            [ria.templates.ModelPropertyBind],
            Number, 'sortType',

            Boolean, function isNotEmptyStatistic(){
                return (this.getItems().filter(function(school){return school.getAvg() !== null}).length +
                    this.getItems().filter(function(school){return school.getInfractionsCount() !== null}).length +
                    this.getItems().filter(function(school){return school.getAbsences() !== null}).length) > 0
            }
        ])
});