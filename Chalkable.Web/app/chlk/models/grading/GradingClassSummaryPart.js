REQUIRE('chlk.models.grading.GradingClassSummaryItems');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryPart*/
    CLASS(
        'GradingClassSummaryPart', [
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items',

            [[ArrayOf(chlk.models.grading.GradingClassSummaryItems)]],
            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
