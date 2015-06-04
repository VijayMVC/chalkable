NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SectionOrder*/
    ENUM('SectionOrder',{
        FULL_SECTION_NUMBER: 0,
        PERIOD_SECTION_NUMBER: 1,
        PERIOD_TEACHER: 2,
        TEACHER_SECTION_NUMBER: 3,
        TEACHER_PERIOD: 4
    });

    /** @class chlk.models.reports.GradeType*/
    ENUM('GradeType',{
        BOTH: 0,
        ALPHA: 1,
        NUMERIC: 2
    });

    /** @class chlk.models.reports.StudentOrder*/
    ENUM('StudentOrder',{
        DISPLAY_NAME: 0,
        GRADE_LEVEL: 1,
        ID_TO_PRINT: 2
    });

    ///** @class chlk.models.reports.IdToPrint*/
    //ENUM('IdToPrint',{
    //    ALT_STUDENT_ID: 0,
    //    SOCIAL_SECURITY_N: 1,
    //    STATE_ID_NUMBER: 2,
    //    STUDENT_NUMBER: 3
    //});

    /** @class chlk.models.reports.SubmitGradeVerificationReportViewData*/
    CLASS('SubmitGradeVerificationReportViewData', EXTENDS(chlk.models.reports.BaseSubmitReportViewData), [

        chlk.models.reports.GradeType, 'gradeType',
        chlk.models.reports.StudentOrder, 'studentOrder',
        //chlk.models.reports.IdToPrint, 'numberToDisplay',

        Boolean, 'includeCommentsAndLegends',
        Boolean, 'includeSignature',
        Boolean, 'includeWithdrawn',
        String, 'gradingPeriodIds',
        String, 'studentAverageIds',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
        }

    ]);
});
