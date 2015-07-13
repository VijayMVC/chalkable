REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');


NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementsByDate*/
    CLASS(
        'AnnouncementsByDate', [
            ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements',

            chlk.models.common.ChlkDate, 'date',

            Number, 'day'
        ]);
});
