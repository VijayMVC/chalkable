REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementPeriod*/
    CLASS(
        'AnnouncementPeriod', EXTENDS(chlk.models.Popup), [
            chlk.models.period.Period, 'period',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            Number, 'index',

            chlk.models.common.ChlkDate, 'date',

            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            chlk.models.id.ClassId, 'selectedClassId'
        ]);
});
