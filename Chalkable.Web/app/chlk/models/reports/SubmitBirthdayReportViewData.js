NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.BirthdayGroupByMethod*/
    ENUM('BirthdayGroupByMethod',{
        NO_GROUPING: 0,
        DATE_OF_BIRTH: 1,
        GRADE_LEVEL: 2,
        HOME_ROOM: 3
    });

    /** @class chlk.models.reports.SubmitBirthdayReportViewData*/
    CLASS('SubmitBirthdayReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.BirthdayGroupByMethod, 'groupBy',

        Number, 'reportFor',
        Number, 'startMonth',
        Number, 'endMonth',
        Boolean, 'includeWithdrawn',
        Boolean, 'includePhoto',
        Boolean, 'saveToFilter',
        Boolean, 'saveAsDefault',
        Number, 'appendOrOverwrite',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
