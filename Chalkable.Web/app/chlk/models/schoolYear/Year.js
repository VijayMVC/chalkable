REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.Year*/
    CLASS(
        'Year', [
            chlk.models.id.SchoolYearId, 'id',
            String, 'description',
            String, 'name',
            [ria.serialize.SerializeProperty('startdate')],
            chlk.models.common.ChlkDate, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            chlk.models.common.ChlkDate, 'endDate',
            [ria.serialize.SerializeProperty('iscurrent')],
            Boolean, 'isCurrent',
            [ria.serialize.SerializeProperty('numberofmarkingperiod')],
            Number, 'numberOfMarkingPeriod'
        ]);
});
