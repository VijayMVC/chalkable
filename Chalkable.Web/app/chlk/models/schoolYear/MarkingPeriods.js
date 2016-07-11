REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";
    /** @class chlk.models.schoolYear.MarkingPeriods*/
    CLASS(
        'MarkingPeriods', [
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'items'
        ]);
});
