REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.grading.GradedItemViewData');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.GradeVerificationReportViewData*/
    CLASS('GradeVerificationReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

        ArrayOf(chlk.models.grading.GradedItemViewData), 'studentAverages',

        Boolean, 'includeWithdrawnStudents',

        [[ArrayOf(chlk.models.schoolYear.GradingPeriod), ArrayOf(chlk.models.grading.GradedItemViewData), ArrayOf(chlk.models.people.ShortUserInfo),
            chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean]],
        function $(gradingPeriods_, studentAverages_, students_, classId_, gradingPeriodId_, startDate_, endDate_, ableDownload_, isAbleToReadSSNumber_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_, students_, ableDownload_, isAbleToReadSSNumber_);
            if(gradingPeriods_)
                this.setGradingPeriods(gradingPeriods_);
            if(studentAverages_)
                this.setStudentAverages(studentAverages_);
        }
    ]);
});