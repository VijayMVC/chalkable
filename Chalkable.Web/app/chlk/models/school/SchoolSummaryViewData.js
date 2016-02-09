REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolYear.Year');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.school.SchoolSummaryViewData*/
    CLASS(
        'SchoolSummaryViewData', [
            chlk.models.admin.BaseStatisticGridViewData, 'itemsStatistic',
            String, 'schoolName',
            chlk.models.id.SchoolId, 'schoolId',
            chlk.models.id.SchoolYearId, 'schoolYearId',
            ArrayOf(chlk.models.schoolYear.Year), 'schoolYears',
            String, 'filter',

            chlk.models.id.SchoolPersonId, 'teacherId',

            [[String, chlk.models.id.SchoolId, chlk.models.id.SchoolYearId, ArrayOf(chlk.models.schoolYear.Year), chlk.models.admin.BaseStatisticGridViewData, String, chlk.models.id.SchoolPersonId]],
            function $(schoolName_, schoolId_, schoolYearId_, schoolYears_, itemsStatistic_, filter_, teacherId_){
                BASE();
                if(schoolName_)
                    this.setSchoolName(schoolName_);
                if(schoolId_)
                    this.setSchoolId(schoolId_);
                if(schoolYearId_)
                    this.setSchoolYearId(schoolYearId_);
                if(schoolYears_)
                    this.setSchoolYears(schoolYears_);
                if(itemsStatistic_)
                    this.setItemsStatistic(itemsStatistic_);
                if(filter_)
                    this.setFilter(filter_);
                if(teacherId_)
                    this.setTeacherId(teacherId_);
            }
        ]);
});
