REQUIRE('chlk.models.id.ScheduleSectionId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.ScheduleSection*/
    CLASS(
        'ScheduleSection', [
            chlk.models.id.ScheduleSectionId, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            String, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            String, 'endDate',
            Number, 'weekdays',
            String, 'name'
        ]);
});
