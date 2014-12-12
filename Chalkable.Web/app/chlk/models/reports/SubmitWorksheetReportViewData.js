REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SubmitWorksheetReportViewData*/
    CLASS('SubmitWorksheetReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        String, 'announcementIds',

        String, 'title1',
        String, 'title2',
        String, 'title3',
        String, 'title4',
        String, 'title5',

        Boolean, 'printAverage',
        Boolean, 'printLetterGrade',
        Boolean, 'printScores',
        Boolean, 'printStudent',
        Boolean, 'workingFilter',
        Boolean, 'appendToExisting',
        Boolean, 'overwriteExisting',

        String, 'submitType',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }
    ]);
});
