REQUIRE('chlk.models.calendar.announcement.DayItem');
REQUIRE('chlk.models.calendar.announcement.Month');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Day*/
    CLASS(
        'Day', EXTENDS(chlk.models.calendar.announcement.Month), [
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items'
        ]);
});
