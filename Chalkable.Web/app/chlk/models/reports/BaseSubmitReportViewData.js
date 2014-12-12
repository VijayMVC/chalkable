REQUIRE('chlk.models.reports.BaseReportViewData');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportFormatEnum */
    ENUM('ReportFormatEnum', {
        PDF: 0,
        CSV: 1,
        EXCEL: 2,
        HTML: 3,
        TIFF: 4,
        XML: 5
    });

    /** @class chlk.models.reports.StudentIdentifierEnum */
    ENUM('StudentIdentifierEnum', {
        NONE: 0,
        STUDENT_NUMBER: 1,
        STATE_ID_NUMBER: 2,
        ALT_STUDENT_NUMBER: 3,
        SOCIAL_SECURITY_NUMBER: 4
    });

    /** @class chlk.models.reports.OrderByEnum */
    ENUM('OrderByEnum', {
        STUDENT_DISPLAY_NAME: 0,
        STUDENT_ID: 1,
        STATE_AVERAGE: 2
    });

    /** @class chlk.models.reports.AttendanceDisplayMethodEnum */
    ENUM('AttendanceDisplayMethodEnum',{
        NONE: 0,
        BOTH: 1,
        GRADING_PERIOD: 2,
        YEAR_TO_DATE: 3
    });

    /** @class chlk.models.reports.BaseSubmitReportViewData*/
    CLASS('BaseSubmitReportViewData', EXTENDS(chlk.models.reports.BaseReportViewData), [

        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',
        chlk.models.reports.ReportFormatEnum, 'format',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }
    ]);
});
