REQUIRE('chlk.models.reports.BaseSubmitReportViewData');
REQUIRE('chlk.models.grading.AlternateScore');

NAMESPACE('chlk.models.reports', function () {
    "use strict";


    /** @class chlk.models.reports.MissingAssignmentsOrderByMethod*/
    ENUM('MissingAssignmentsOrderByMethod',{
        STUDENT_IDENTIFIER: 0,
        SECTION_NUMBER: 1
    });

    /** @class chlk.models.reports.SubmitMissingAssignmentsReportViewData*/

    CLASS('SubmitMissingAssignmentsReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [


        Boolean, 'alternateScoresOnly',
        Boolean, 'considerZerosAsMissingGrades',
        Boolean, 'includeWithdrawnStudents',
        Boolean, 'suppressStudentName',
        Boolean, 'onePerPage',

        chlk.models.reports.MissingAssignmentsOrderByMethod, 'orderBy',

        String, 'alternateScoreIds',

        ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

        [[  chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            ArrayOf(chlk.models.people.ShortUserInfo), ArrayOf(chlk.models.grading.AlternateScore), Boolean, Boolean
        ]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_, students_, alternateScores_, ableDownload_, isAbleToReadSSN_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_, students_, ableDownload_, isAbleToReadSSN_);
            if(alternateScores_)
                this.setAlternateScores(alternateScores_);
        }
    ]);
});
