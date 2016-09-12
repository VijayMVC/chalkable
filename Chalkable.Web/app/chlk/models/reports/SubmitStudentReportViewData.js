REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.SubmitStudentReportViewData*/
    CLASS('SubmitStudentReportViewData', [

        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

        chlk.models.id.SchoolPersonId, 'studentId',

        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        Boolean, 'ableDownload',

        [[ArrayOf(chlk.models.schoolYear.GradingPeriod), chlk.models.id.SchoolPersonId, Boolean]],
        function $(gradingPeriods_, studentId_, ableDownload_){
            BASE();
            if(gradingPeriods_)
                this.setGradingPeriods(gradingPeriods_);
            if(studentId_)
                this.setStudentId(studentId_);
            if (ableDownload_)
                this.setAbleDownload(ableDownload_);
        }

    ]);
});
