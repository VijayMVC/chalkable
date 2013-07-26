REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.MarkingPeriod*/
    CLASS(
        'MarkingPeriod', [
            chlk.models.id.MarkingPeriodId, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            String, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            String, 'endDate',
            Number, 'weekdays',
            String, 'name'
        ]);
});
