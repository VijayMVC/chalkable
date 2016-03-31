REQUIRE('chlk.models.reports.BaseSubmitReportViewData');
REQUIRE('chlk.models.reports.UserForReport');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SubmitProgressReportViewData*/
    CLASS('SubmitProgressReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        Number, 'notSelectedCount',

        String, 'commentsList',

        String, 'studentComments',

        String, 'absenceReasonIds',

        Boolean, 'additionalMailings',

        chlk.models.reports.AttendanceDisplayMethodEnum, 'dailyAttendanceDisplayMethod',

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

        ArrayOf(chlk.models.reports.UserForReport), 'studentsWithComments',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

        [[ArrayOf(chlk.models.attendance.AttendanceReason),
            ArrayOf(chlk.models.reports.UserForReport),
            chlk.models.id.GradingPeriodId, chlk.models.id.ClassId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean
        ]],
        function $(reasons_, students_, gradingPeriodId_, classId_, startDate_, endDate_, ableDownload_, isAbleToReadSSNumber_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_, null, ableDownload_, isAbleToReadSSNumber_);
            if(students_)
                this.setStudentsWithComments(students_);
            if(reasons_)
                this.setReasons(reasons_);
        }
    ]);
});
