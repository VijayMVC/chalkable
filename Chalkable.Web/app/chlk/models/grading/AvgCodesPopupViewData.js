REQUIRE('chlk.models.grading.AvgCodeHeaderViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.AvgCodesPopupViewData*/
    CLASS(
        'AvgCodesPopupViewData', [
            ArrayOf(chlk.models.grading.AvgCodeHeaderViewData), 'headers',

            ArrayOf(chlk.models.grading.AvgComment), 'comments',

            String, 'studentName',

            String, 'gradingPeriodName',

            Number, 'averageId',

            Number, 'rowIndex',

            [[ArrayOf(chlk.models.grading.AvgCodeHeaderViewData), ArrayOf(chlk.models.grading.AvgComment), String, String, Number]],
            function $(headers_, comments_, studentName_, gradingPeriodName_, averageId_, rowIndex_){
                BASE();
                if(headers_)
                    this.setHeaders(headers_);
                if(comments_)
                    this.setComments(comments_);
                if(studentName_)
                    this.setStudentName(studentName_);
                if(gradingPeriodName_)
                    this.setGradingPeriodName(gradingPeriodName_);
                if(averageId_ || rowIndex_ == 0)
                    this.setAverageId(averageId_);
                if(rowIndex_ || rowIndex_ == 0)
                    this.setRowIndex(rowIndex_);
            }
    ]);
});
