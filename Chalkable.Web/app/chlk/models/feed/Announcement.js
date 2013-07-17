NAMESPACE('chlk.models.feed', function () {
    "use strict";

    /** @class chlk.models.feed.AnnouncementId*/
    IDENTIFIER('AnnouncementId');

    /** @class chlk.models.feed.Announcement*/
    CLASS(
        'Announcement', [
            chlk.models.feed.AnnouncementId, 'id'
        ]);
});
