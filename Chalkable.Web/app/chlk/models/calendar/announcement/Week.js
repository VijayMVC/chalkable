REQUIRE('chlk.models.calendar.announcement.WeekItem');
REQUIRE('chlk.models.calendar.announcement.Month');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Week*/
    CLASS(
        'Week', EXTENDS(chlk.models.calendar.announcement.Month), [
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items'
        ]);
});
