REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.school.SchoolStatistic');

NAMESPACE('chlk.models.district', function () {
    "use strict";

    /** @class chlk.models.district.DistrictFullSummaryViewData*/
    CLASS(
        'DistrictFullSummaryViewData', [
            chlk.models.common.PaginatedList, 'schoolsStatistic',
            chlk.models.district.DistrictShortSummaryViewData, 'shortSummary',

            function $(shortSummary_, schoolsStatistic_){
                BASE();
                if(shortSummary_)
                    this.setShortSummary(shortSummary_);
                if(schoolsStatistic_)
                    this.setSchoolsStatistic(schoolsStatistic_);
            }
        ]);
});
