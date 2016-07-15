NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.BirthdayReportViewData*/
    CLASS('BirthdayReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(gradingPeriodId_, classId_, startDate_, endDate_);
        }
    ]);
});