REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.GradingPeriodId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.GradingPeriod*/
    CLASS(
        'GradingPeriod', [
            chlk.models.id.GradingPeriodId, 'id',
            String, 'name',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            chlk.models.common.ChlkDate, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            chlk.models.common.ChlkDate, 'endDate'
        ]);
});
