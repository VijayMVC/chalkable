REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.CopyAnnouncementResultViewData*/
    CLASS('CopyAnnouncementResultViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
        VOID, function deserialize(raw){
            this.announcementType = SJX.fromValue(raw.announcementtype, chlk.models.announcement.AnnouncementTypeEnum);
            this.announcementId = SJX.fromValue(raw.announcementId, chlk.models.id.AnnouncementId);
        },

        chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

        chlk.models.id.AnnouncementId, 'announcementId'
    ]);
});
