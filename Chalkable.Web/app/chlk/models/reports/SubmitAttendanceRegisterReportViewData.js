NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportType*/
    ENUM('ReportType',{
        BOTH: 0,
        DETAIL: 1,
        SUMMARY: 2
    });

    /** @class chlk.models.reports.SubmitAttendanceRegisterReportViewData*/
    CLASS('SubmitAttendanceRegisterReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.ReportType, 'reportType',

        String, 'absenceReasons',
        Number, 'monthId',
        Boolean, 'showLocalReasonCode',
        Boolean, 'includeTardies',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
