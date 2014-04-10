REQUIRE('chlk.models.grading.GradingClassStandardsItems');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryPart*/
    CLASS(
        'GradingClassSummaryPart', [
            ArrayOf(chlk.models.grading.GradingClassStandardsItems), 'items',

            [[ArrayOf(chlk.models.grading.GradingClassStandardsItems)]],
            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
