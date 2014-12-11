REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SubmitGradeBookReportViewData*/
    CLASS('SubmitGradeBookReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        Number, 'reportType',

        chlk.models.reports.OrderByEnum, 'orderBy',

        Boolean, 'displayLetterGrade',
        Boolean, 'displayTotalPoints',
        Boolean, 'displayStudentAverage',
        Boolean, 'includeWithdrawnStudents',
        Boolean, 'includeNonGradedActivities',
        Boolean, 'suppressStudentName',

        String, 'submitType',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
