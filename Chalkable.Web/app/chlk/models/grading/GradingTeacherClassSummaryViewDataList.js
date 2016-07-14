REQUIRE('chlk.models.grading.GradingTeacherClassSummaryViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingTeacherClassSummaryViewDataList*/
    CLASS(
        'GradingTeacherClassSummaryViewDataList', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.grading.GradingTeacherClassSummaryViewData), 'items',

            [[chlk.models.classes.ClassesForTopBar, ArrayOf(chlk.models.grading.GradingTeacherClassSummaryViewData)]],
            function $(topData_, items_){
                BASE(topData_);
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
