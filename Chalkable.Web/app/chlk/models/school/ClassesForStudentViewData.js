REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.admin.BaseStatisticGridViewData');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.school.ClassesForStudentViewData*/
    CLASS(
        'ClassesForStudentViewData', [
            chlk.models.admin.BaseStatisticGridViewData, 'itemsStatistic',
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            [[chlk.models.id.GradingPeriodId, ArrayOf(chlk.models.schoolYear.GradingPeriod, chlk.models.admin.BaseStatisticGridViewData)]],
            function $(gradingPeriodId_, gradingPeriods_, itemsStatistic_){
                BASE();
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
                if(gradingPeriods_)
                    this.setGradingPeriods(gradingPeriods_);
                if(itemsStatistic_)
                    this.setItemsStatistic(itemsStatistic_);
            }
        ]);
});
