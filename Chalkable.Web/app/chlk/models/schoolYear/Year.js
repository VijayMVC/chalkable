NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.Year*/
    CLASS(
        'Year', [
            Number, 'id',
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
