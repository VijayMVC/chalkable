REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.school.SchoolClassesSummaryViewData*/
    CLASS(
        'SchoolClassesSummaryViewData', [
            chlk.models.common.PaginatedList, 'classesStatistic',
            String, 'schoolName',
            chlk.models.id.SchoolId, 'schoolId',
            String, 'filter',

            [[String, chlk.models.id.SchoolId, chlk.models.common.PaginatedList, String]],
            function $(schoolName_, schoolId_, classesStatistic_, filter_){
                BASE();
                if(schoolName_)
                    this.setSchoolName(schoolName_);
                if(schoolId_)
                    this.setSchoolId(schoolId_);
                if(classesStatistic_)
                    this.setClassesStatistic(classesStatistic_);
                if(filter_)
                    this.setFilter(filter_);
            }
        ]);
});
