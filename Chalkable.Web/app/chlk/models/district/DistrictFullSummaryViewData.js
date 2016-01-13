REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');

NAMESPACE('chlk.models.district', function () {
    "use strict";

    /** @class chlk.models.district.DistrictFullSummaryViewData*/
    CLASS(
        'DistrictFullSummaryViewData', [
            chlk.models.admin.BaseStatisticGridViewData, 'schoolsStatistic',
            chlk.models.district.DistrictShortSummaryViewData, 'shortSummary',
            String, 'filter',

            function $(shortSummary_, schoolsStatistic_, filter_){
                BASE();
                if(shortSummary_)
                    this.setShortSummary(shortSummary_);
                if(schoolsStatistic_)
                    this.setSchoolsStatistic(schoolsStatistic_);
                if(filter_)
                    this.setFilter(filter_);
            }
        ]);
});
