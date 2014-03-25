REQUIRE('chlk.models.grading.GradingClassSummaryItem');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItems*/
    CLASS(
        'GradingClassSummaryItems', [
            ArrayOf(chlk.models.grading.GradingClassSummaryItem), 'items',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.common.NameId, 'gradingPeriod',

            Number, 'avg'
        ]);
});
