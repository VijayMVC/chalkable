REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryGridViewData*/
    CLASS(
        'GradingClassSummaryGridViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), 'items',

            chlk.models.grading.Mapping, 'gradingStyleMapper',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId,
                ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), chlk.models.grading.Mapping]],
            function $(topData_, selectedId_, items_, mapping_){
                BASE(topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
                if(mapping_)
                    this.setGradingStyleMapper(mapping_);
            }
        ]);
});