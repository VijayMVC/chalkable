REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortAnnouncementViewData*/
    CLASS(
        'ShortAnnouncementViewData', EXTENDS(chlk.models.announcement.BaseAnnouncementViewData), [
            [ria.serialize.SerializeProperty('studentannouncements')],
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements'
        ]);
});
