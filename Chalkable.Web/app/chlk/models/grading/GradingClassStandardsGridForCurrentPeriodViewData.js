REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.standard.StandardGradings');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassStandardsGridForCurrentPeriodViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            [ria.serialize.SerializeProperty('gradingperiods')],
            ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

            Boolean, 'ableEdit',

            [ria.serialize.SerializeProperty('currentstandardgradinggrid')],
            chlk.models.grading.GradingClassSummaryGridItems.OF(chlk.models.standard.StandardGradings), 'currentGradingGrid',

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades'
        ]);
});