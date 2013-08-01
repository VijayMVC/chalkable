REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.MarkingPeriod*/
    CLASS(
        'MarkingPeriod', [
            chlk.models.id.MarkingPeriodId, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            chlk.models.common.ChlkDate, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            chlk.models.common.ChlkDate, 'endDate',
            Number, 'weekdays',
            String, 'name'
        ]);
});
