REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.admin.BaseStatistic');

NAMESPACE('chlk.templates.district', function () {

    /** @class chlk.templates.district.BaseStatisticTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/district/DistrictSummaryGrid.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'BaseStatisticTpl', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.admin.BaseStatistic), 'items',

            Boolean, function isNotEmptyStatistic(){
                return (this.getItems().filter(function(school){return school.getAvg() !== null}).length +
                    this.getItems().filter(function(school){return school.getInfractionsCount() !== null}).length +
                    this.getItems().filter(function(school){return school.getAbsences() !== null}).length) > 0
            }
        ])
});