REQUIRE('chlk.models.grading.StudentWithAvg');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradingClassSummaryGridItems*/
    CLASS(
        GENERIC('TItem'),
        'GradingClassSummaryGridItems', [
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.serialize.SerializeProperty('gradingitems')],
            ArrayOf(TItem), 'gradingItems',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.common.NameId, 'gradingPeriod',

            Number, 'avg'
        ]);
});
