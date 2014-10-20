REQUIRE('chlk.models.grading.GradingClassSummaryPart');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummary*/
    CLASS(
        'GradingClassSummary', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

            String, 'action',

            String, 'gridAction',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, chlk.models.grading.GradingClassSummaryPart]],
            function $(topData_, selectedId_, summaryPart_){
                BASE(topData_, selectedId_);
                if(summaryPart_)
                    this.setSummaryPart(summaryPart_);
            }
        ]);
});
