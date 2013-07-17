NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.MarkingPeriod*/
    CLASS(
        'MarkingPeriod', [
            Number, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            String, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            String, 'endDate',
            Number, 'weekdays',
            String, 'name'
        ]);
});
