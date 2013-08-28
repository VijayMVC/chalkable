REQUIRE('chlk.models.announcement.AnnouncementClassPeriod');
REQUIRE('chlk.models.period.Period');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.CalendarDayItem*/
    CLASS(
        'CalendarDayItem', EXTENDS(chlk.models.Popup), [
            [ria.serialize.SerializeProperty('announcementclassperiods')],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'announcementClassPeriods',

            chlk.models.period.Period, 'period',

            chlk.models.common.ChlkDate, 'date'
        ]);
});
