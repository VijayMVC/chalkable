REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.Announcement');


NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementsByDate*/
    CLASS(
        'AnnouncementsByDate', [
            ArrayOf(chlk.models.announcement.Announcement), 'announcements',

            chlk.models.common.ChlkDate, 'date',

            Number, 'day'
        ]);
});
