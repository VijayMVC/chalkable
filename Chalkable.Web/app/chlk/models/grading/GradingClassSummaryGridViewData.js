REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridViewData*/
    CLASS(
        'GradingClassSummaryGridViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), 'items',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.grading.GradingClassSummaryGridItems)]],
            function $(topData_, selectedId_, items_){
                BASE(topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
            }
        ]);
});