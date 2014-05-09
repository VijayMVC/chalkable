REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementTypeGradingId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingSummaryGridSubmitViewData*/
    CLASS(
        'GradingSummaryGridSubmitViewData', [
            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            chlk.models.id.StandardId, 'standardId',

            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            chlk.models.id.ClassId, 'classId',

            Boolean, 'autoUpdate',

            Boolean, 'notCalculateGrid',

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, Boolean, Boolean]],
            function $(classId_, gradingPeriodId_, notCalculateGrid_, isAutoUpdate_){
                BASE();
                if(classId_)
                    this.setClassId(classId_);
                if(gradingPeriodId_)
                    this.setGradingPeriodId(gradingPeriodId_);
                this.setNotCalculateGrid(notCalculateGrid_ || false);
                this.setAutoUpdate(isAutoUpdate_ || false);
            }
        ]);
});