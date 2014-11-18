REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortAnnouncementViewData*/
    CLASS(
        UNSAFE, 'ShortAnnouncementViewData', EXTENDS(chlk.models.announcement.BaseAnnouncementViewData), [
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements',

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.studentAnnouncements = ria.serialize.SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.ShortStudentAnnouncementsViewData);
            }
        ]);
});
