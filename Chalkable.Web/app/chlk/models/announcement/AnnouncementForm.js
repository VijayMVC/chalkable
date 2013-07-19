REQUIRE('chlk.models.class.ClassesForTopBar');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementForm*/
    CLASS(
        'AnnouncementForm', EXTENDS(chlk.models.announcement.Announcement), [
            chlk.models.class.ClassesForTopBar, 'topData'
        ]);
});
