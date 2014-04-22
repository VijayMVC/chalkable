REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.SubmitGradeBookReportViewData*/
    CLASS('SubmitGradeBookReportViewData', [
        chlk.models.id.ClassId, 'classId',

        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        chlk.models.common.ChlkDate, 'startDate',

        chlk.models.common.ChlkDate, 'endDate',

        Number, 'reportType',

        Number, 'orderBy',

        Number, 'idToPrint',

        Number, 'format',

        Boolean, 'displayLetterGrade',

        Boolean, 'displayTotalPoints',

        Boolean, 'displayStudentAverage',

        Boolean, 'includeWithdrawnStudents',

        Boolean, 'includeNonGradedActivities'
    ]);
});
