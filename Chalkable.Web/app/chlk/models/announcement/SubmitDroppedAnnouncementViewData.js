REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.SubmitDroppedAnnouncementViewData*/
    CLASS(
        'SubmitDroppedAnnouncementViewData', [
            chlk.models.id.AnnouncementId, 'announcementId',
            Boolean, 'dropped'
        ]);
});
