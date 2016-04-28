REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.grading.StudentAverageInfo');
REQUIRE('chlk.models.attendance.StudentFinalAttendanceSummaryViewData');
REQUIRE('chlk.models.grading.StudentGradingByTypeStatsViewData');
REQUIRE('chlk.models.discipline.DisciplineTypeSummaryViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StudentFinalGradeViewData*/
    CLASS('StudentFinalGradeViewData', [
        chlk.models.people.ShortUserInfo, 'student',

        [ria.serialize.SerializeProperty('currentstudentaverage')],
        chlk.models.grading.ShortStudentAverageInfo, 'currentStudentAverage',

        [ria.serialize.SerializeProperty('studentaverages')],
        ArrayOf(chlk.models.grading.ShortStudentAverageInfo), 'studentAverages',

        chlk.models.attendance.StudentFinalAttendanceSummaryViewData, 'attendance',

        ArrayOf(chlk.models.discipline.DisciplineTypeSummaryViewData), 'disciplines',

        [ria.serialize.SerializeProperty('statsbytype')],
        ArrayOf(chlk.models.grading.StudentGradingByTypeStatsViewData), 'statsByType'
    ]);
});
