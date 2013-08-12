REQUIRE('chlk.models.id.PeriodId');
REQUIRE('chlk.models.id.MarkingPeriodId');
NAMESPACE('chlk.models.period', function () {
    "use strict";

    /** @class chlk.models.period.Period*/
    CLASS(
        'Period', [
            chlk.models.id.PeriodId, 'id',

            [ria.serialize.SerializeProperty('starttime')],
            Number, 'startTime',

            [ria.serialize.SerializeProperty('endtime')],
            Number, 'endTime',

            Number, 'order',

            [ria.serialize.SerializeProperty('markingperiodid')],
            chlk.models.id.MarkingPeriodId, 'markingPeriodId'
        ]);
});
