REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummary*/
    CLASS(
        'GradingClassSummary', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items'
        ]);
});
