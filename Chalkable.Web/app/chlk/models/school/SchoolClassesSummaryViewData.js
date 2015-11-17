REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolYear.Year');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.school.SchoolClassesSummaryViewData*/
    CLASS(
        'SchoolClassesSummaryViewData', [
            chlk.models.common.PaginatedList, 'classesStatistic',
            String, 'schoolName',
            chlk.models.id.SchoolId, 'schoolId',
            chlk.models.id.SchoolYearId, 'schoolYearId',
            ArrayOf(chlk.models.schoolYear.Year), 'schoolYears',
            String, 'filter',

            [[String, chlk.models.id.SchoolId, chlk.models.id.SchoolYearId, ArrayOf(chlk.models.schoolYear.Year), chlk.models.common.PaginatedList, String]],
            function $(schoolName_, schoolId_, schoolYearId_, schoolYears_, classesStatistic_, filter_){
                BASE();
                if(schoolName_)
                    this.setSchoolName(schoolName_);
                if(schoolId_)
                    this.setSchoolId(schoolId_);
                if(schoolYearId_)
                    this.setSchoolYearId(schoolYearId_);
                if(schoolYears_)
                    this.setSchoolYears(schoolYears_);
                if(classesStatistic_)
                    this.setClassesStatistic(classesStatistic_);
                if(filter_)
                    this.setFilter(filter_);
            }
        ]);
});
