REQUIRE('chlk.models.reports.BaseReportViewData');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SubmitComprehensiveProgressViewData*/

    CLASS('SubmitComprehensiveProgressViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        Number, 'orderBy',

        String, 'gradingPeriodIds',

        String, 'absenceReasonIds',

        Boolean, 'additionalMailings',

        Number, 'dailyAttendanceDisplayMethod',

        Boolean, 'classAverageOnly',

        Boolean, 'displayCategoryAverages',

        Boolean, 'displayClassAverages',

        Boolean, 'displayPeriodAttendance',

        Boolean, 'displaySignatureLine',

        Boolean, 'displayStudentComments',

        Boolean, 'displayStudentMailingAddress',

        Boolean, 'displayTotalPoints',

        Boolean, 'goGreen',

        Number, 'maxStandardAverage',

        Number, 'minStandardAverage',

        Boolean, 'includePicture',

        Boolean, 'includeWithdrawnStudents',

        Boolean, 'windowEnvelope',

        chlk.models.id.SchoolPersonId, 'studentFilterId',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',
        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            ArrayOf(chlk.models.schoolYear.GradingPeriod),
            ArrayOf(chlk.models.attendance.AttendanceReason),
        ]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_, gradingPeriods_, reasons_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
            if(gradingPeriods_)
                this.setGradingPeriods(gradingPeriods_);
            if(reasons_)
                this.setReasons(reasons_);
        }
    ]);
});
