REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.ShortAnnouncementForProfileViewData*/
    CLASS(
        UNSAFE, 'ShortAnnouncementForProfileViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements',

            chlk.models.id.AnnouncementId, 'id',
            String, 'title',
            Boolean, 'owner',
            chlk.models.announcement.AnnouncementTypeEnum, 'type',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(Number(raw.id), chlk.models.id.AnnouncementId);
                this.title = SJX.fromValue(raw.title, String);
                this.type = SJX.fromValue(raw.type, chlk.models.announcement.AnnouncementTypeEnum);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.studentAnnouncements = ria.serialize.SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.ShortStudentAnnouncementsViewData);
            }
        ]);
});
