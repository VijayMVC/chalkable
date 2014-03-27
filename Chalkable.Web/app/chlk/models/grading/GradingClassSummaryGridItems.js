REQUIRE('chlk.models.grading.StudentWithAvg');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradingClassSummaryGridItems*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridItems', [
            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.common.NameId, 'gradingPeriod',

            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.serialize.SerializeProperty('gradingitems')],
            ArrayOf(TItem), 'gradingItems',

            Number, 'avg'
        ]);
});
