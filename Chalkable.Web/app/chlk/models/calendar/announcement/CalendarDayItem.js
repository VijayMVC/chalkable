REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');
REQUIRE('chlk.models.period.Period');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.CalendarDayItem*/
    CLASS(
        'CalendarDayItem', [
            [ria.serialize.SerializeProperty('announcementclassperiods')],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'announcementClassPeriods',

            chlk.models.period.Period, 'period'
        ]);
});
