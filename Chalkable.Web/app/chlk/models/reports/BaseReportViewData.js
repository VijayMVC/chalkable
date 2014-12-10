REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.BaseReportViewData*/

    CLASS('BaseReportViewData', [

        chlk.models.id.ClassId, 'classId',
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',
        chlk.models.common.ChlkDate, 'startDate',
        chlk.models.common.ChlkDate, 'endDate',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE();
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
