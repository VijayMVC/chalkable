NAMESPACE('chlk.models.grading', function (){
   "use strict";

    /**@class chlk.models.grading.GradeBookReportViewData*/
    CLASS('GradeBookReportViewData',  [
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        chlk.models.id.ClassId, 'classId',

        [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId]],
        function $(gradingPeriodId_, classId_){
            BASE();
            if(gradingPeriodId_)
                this.setGradingPeriodId(gradingPeriodId_);
            if(classId_)
                this.setClassId(classId_);
        }
    ]);
});