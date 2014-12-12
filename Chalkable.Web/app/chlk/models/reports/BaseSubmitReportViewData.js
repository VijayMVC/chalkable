REQUIRE('chlk.models.reports.BaseReportViewData');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.BaseSubmitReportViewData*/

    CLASS('BaseSubmitReportViewData', EXTENDS(chlk.models.reports.BaseReportViewData), [

        Number, 'idToPrint',
        Number, 'format',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }
    ]);
});
