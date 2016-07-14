REQUIRE('chlk.models.schoolYear.ScheduleSection');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.ScheduleSectionsForMP*/
    CLASS(
        'ScheduleSectionsForMP', [
            ArrayOf(chlk.models.schoolYear.ScheduleSection), 'sections',
            Number, 'weekdays',
            [ria.serialize.SerializeProperty('canchange')],
            Boolean, 'canChange',
            Boolean, 'equivalent'
        ]);
});
