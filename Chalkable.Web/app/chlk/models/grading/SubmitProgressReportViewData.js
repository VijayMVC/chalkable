REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.SubmitProgressReportViewData*/
    CLASS('SubmitProgressReportViewData', [
        chlk.models.id.ClassId, 'classId',

        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        chlk.models.common.ChlkDate, 'startDate',

        chlk.models.common.ChlkDate, 'endDate',

        Number, 'idToPrint',

        Number, 'format',

        String, 'absenceReasonIds',

        Boolean, 'additionalMailings',

        Number, 'dailyAttendanceDisplayMethod',

        Boolean, 'displayCategoryAverages',

        Boolean, 'displayClassAverages',

        Boolean, 'displayLetterGrade',

        Boolean, 'displayPeriodAttendance',

        Boolean, 'displaySignatureLine',

        Boolean, 'displayStudentComments',

        Boolean, 'displayStudentMailingAddress',

        Boolean, 'displayTotalPoints',

        Boolean, 'goGreen',

        Number, 'maxCategoryClassAverage',

        Number, 'maxStandardAverage',

        Number, 'minCategoryClassAverage',

        Number, 'minStandardAverage',

        Boolean, 'printFromHomePortal',

        String, 'classComment',

        String, 'studentIds',

        ArrayOf(chlk.models.people.ShortUserInfo), 'students',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

        [[ArrayOf(chlk.models.attendance.AttendanceReason), ArrayOf(chlk.models.people.ShortUserInfo), chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(reasons_, students_, gradingPeriodId_, classId_, startDate_, endDate_){
            BASE();
            if(students_)
                this.setStudents(students_);
            if(reasons_)
                this.setReasons(reasons_);
            if(gradingPeriodId_)
                this.setGradingPeriodId(gradingPeriodId_);
            if(classId_)
                this.setClassId(classId_);
            if(startDate_)
                this.setStartDate(startDate_);
            if(endDate_)
                this.setEndDate(endDate_);
        }
    ]);
});
