REQUIRE('chlk.models.reports.BaseSubmitReportViewData');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ComprehensiveProgressOrderByMethod*/
    ENUM('ComprehensiveProgressOrderByMethod', {
        STUDENT_DISPLAY_NAME: 1,
        STUDNET_IDENTIFIER: 2,
        GRADE_LEVEL: 3,
        HOME_ROOM: 4,
        POSTAL_CODE: 5,
        DISTRIBUTION_PERIOD: 6
    });

    /** @class chlk.models.reports.SubmitComprehensiveProgressViewData*/

    CLASS('SubmitComprehensiveProgressViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.ComprehensiveProgressOrderByMethod, 'orderBy',

        String, 'gradingPeriodIds',

        String, 'absenceReasonIds',

        Boolean, 'additionalMailings',

        chlk.models.reports.AttendanceDisplayMethodEnum, 'dailyAttendanceDisplayMethod',

        Boolean, 'classAverageOnly',

        Boolean, 'displayCategoryAverages',

        Boolean, 'displayClassAverages',

        Boolean, 'displayPeriodAttendance',

        Boolean, 'displaySignatureLine',

        Boolean, 'displayStudentComment',

        Boolean, 'displayStudentMailingAddress',

        Boolean, 'displayTotalPoints',

        Boolean, 'goGreen',

        Number, 'maxStandardAverage',

        Number, 'minStandardAverage',

        Boolean, 'includePicture',

        Boolean, 'includeWithdrawnStudents',

        Boolean, 'windowEnvelope',

        String, 'studentIds',

        chlk.models.id.SchoolPersonId, 'studentFilterId',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',
        ArrayOf(chlk.models.people.User), 'students',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            ArrayOf(chlk.models.attendance.AttendanceReason),
            ArrayOf(chlk.models.people.User)
        ]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_,  reasons_, students_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
            if(reasons_)
                this.setReasons(reasons_);
            if(students_)
                this.setStudents(students_);
        }
    ]);
});
