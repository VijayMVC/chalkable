REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.SubmitWorksheetReportViewData*/
    CLASS('SubmitWorksheetReportViewData', [
        chlk.models.id.ClassId, 'classId',

        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        chlk.models.common.ChlkDate, 'startDate',

        chlk.models.common.ChlkDate, 'endDate',

        String, 'announcementIds',

        String, 'title1',

        String, 'title2',

        String, 'title3',

        String, 'title4',

        String, 'title5',

        Number, 'idToPrint',

        Number, 'format',

        Boolean, 'printAverage',

        Boolean, 'printLetterGrade',

        Boolean, 'printScores',

        Boolean, 'printStudent',

        Boolean, 'workingFilter',

        Boolean, 'appendToExisting',

        Boolean, 'overwriteExisting',

        String, 'submitType'
    ]);
});
