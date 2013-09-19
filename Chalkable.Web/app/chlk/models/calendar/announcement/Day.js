REQUIRE('chlk.models.calendar.announcement.DayItem');
REQUIRE('chlk.models.calendar.BaseCalendar');


NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Day*/
    CLASS(
        'Day', EXTENDS(chlk.models.calendar.BaseCalendar),  [
            chlk.models.classes.ClassesForTopBar, 'topData',
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items'
        ]);
});
