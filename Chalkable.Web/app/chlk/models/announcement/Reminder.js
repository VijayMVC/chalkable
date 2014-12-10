REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.Reminder*/
    CLASS(
        UNSAFE, FINAL, 'Reminder', IMPLEMENTS(ria.serialize.IDeserializable),  [

            VOID, function deserialize(raw) {
                this.before = SJX.fromValue(raw.before, Number);
                this.id = SJX.fromValue(raw.id, chlk.models.id.ReminderId);
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.owner = SJX.fromValue(raw.isowner, Boolean);
                this.remindDate = SJX.fromDeserializable(raw.reminddate, chlk.models.common.ChlkDate);
                this.duplicate = SJX.fromValue(raw.duplicate, Boolean);
            },

            Number, 'before',
            chlk.models.id.ReminderId, 'id',
            chlk.models.id.AnnouncementId, 'announcementId',
            Boolean, 'owner',
            chlk.models.common.ChlkDate, 'remindDate',
            Boolean, 'duplicate'
        ]);
});
