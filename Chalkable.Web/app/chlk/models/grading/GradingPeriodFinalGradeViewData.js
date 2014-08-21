REQUIRE('chlk.models.grading.StudentFinalGradeViewData');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradingPeriodFinalGradeViewData*/
    CLASS('GradingPeriodFinalGradeViewData', [
        [ria.serialize.SerializeProperty('studentfinalgrades')],
        ArrayOf(chlk.models.grading.StudentFinalGradeViewData), 'studentFinalGrades',

        [ria.serialize.SerializeProperty('currentaverage')],
        chlk.models.grading.StudentAverageInfo, 'currentAverage',

        ArrayOf(chlk.models.grading.StudentAverageInfo), 'averages',

        [ria.serialize.SerializeProperty('gradingperiod')],
        chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

        Number, 'selectedIndex',

        Boolean, 'avgChanged'
    ]);
});
