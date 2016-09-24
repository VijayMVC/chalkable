NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.GroupBy*/
    ENUM('GroupBy',{
        BLANK_COLUMN: 0,
        GRADE_LEVEL_SEQUENCE: 1,
        HOMEROOM_NAME: 3
    });

    /** @class chlk.models.reports.SubmitAttendanceProfileReportViewData*/
    CLASS('SubmitAttendanceProfileReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.GroupBy, 'groupBy',

        String, 'absenceReasons',
        String, 'terms',
        Boolean, 'displayPeriodAbsences',
        Boolean, 'displayReasonTotals',
        Boolean, 'includeCheck',
        Boolean, 'includeUnlisted',
        Boolean, 'displayNote',
        Boolean, 'displayWithdrawnStudents',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
