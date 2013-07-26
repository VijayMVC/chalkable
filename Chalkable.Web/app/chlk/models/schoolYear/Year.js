REQUIRE('chlk.models.id.SchoolYearId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.Year*/
    CLASS(
        'Year', [
            chlk.models.id.SchoolYearId, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            String, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            String, 'endDate',
            [ria.serialize.SerializeProperty('iscurrent')],
            Boolean, 'isCurrent',
            [ria.serialize.SerializeProperty('numberofmarkingperiod')],
            Number, 'numberOfMarkingPeriod'
        ]);
});
