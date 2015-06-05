REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.GroupId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AdminAnnouncementRecipient*/
    CLASS(
        'AdminAnnouncementRecipient', IMPLEMENTS(ria.serialize.IDeserializable), [

            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.serialize.SerializeProperty('groupid')],
            chlk.models.id.GroupId, 'groupId',

            [ria.serialize.SerializeProperty('groupname')],
            String, 'groupName',

             VOID, function deserialize(raw){
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.groupId = SJX.fromValue(raw.groupid, chlk.models.id.GroupId);
                this.groupName = SJX.fromValue(raw.groupname, String);
            }

        ]);
});
