REQUIRE('chlk.models.grading.GradingClassSummaryPart');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.common.PageWithClassesAndGradingPeriodsViewData');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummary*/
    CLASS(
        'GradingClassSummary', EXTENDS(chlk.models.common.PageWithClassesAndGradingPeriodsViewData), [
            chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

            Boolean, 'inProfile',

            chlk.models.id.ClassId, 'classId',

            Boolean, 'hasAccessToLE',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, chlk.models.grading.GradingClassSummaryPart, Boolean]],
            function $(topData_, selectedId_, summaryPart_, hasAccessToLE_){
                BASE(topData_, selectedId_);
                if(hasAccessToLE_)
                    this.setHasAccessToLE(hasAccessToLE_);
                if(summaryPart_)
                    this.setSummaryPart(summaryPart_);
            }
        ]);
});
