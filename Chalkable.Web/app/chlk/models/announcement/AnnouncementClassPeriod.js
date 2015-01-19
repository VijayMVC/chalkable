REQUIRE('chlk.models.period.ClassPeriod');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementClassPeriod*/
    CLASS(
        'AnnouncementClassPeriod', [
            [ria.serialize.SerializeProperty('classperiod')],
            chlk.models.period.ClassPeriod, 'classPeriod',

            [ria.serialize.SerializeProperty('daynumber')],
            Number, 'dayNumber',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements'
        ]);
});
