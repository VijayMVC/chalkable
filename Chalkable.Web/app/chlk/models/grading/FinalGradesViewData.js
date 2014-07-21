REQUIRE('chlk.models.grading.GradingPeriodFinalGradeViewData');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.FinalGradesViewData*/
    CLASS('FinalGradesViewData', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
        [ria.serialize.SerializeProperty('currentfinalgrade')],
        chlk.models.grading.GradingPeriodFinalGradeViewData, 'currentFinalGrade',

        [ria.serialize.SerializeProperty('gradingperiods')],
        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

        ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

        ArrayOf(chlk.models.grading.AvgComment), 'gradingComments'
    ]);
});
