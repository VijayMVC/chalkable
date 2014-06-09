REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementsViewData*/
    CLASS(
        'ShortStudentAnnouncementsViewData', [
            ArrayOf(chlk.models.announcement.ShortStudentAnnouncementViewData), 'items'
        ]);
});
