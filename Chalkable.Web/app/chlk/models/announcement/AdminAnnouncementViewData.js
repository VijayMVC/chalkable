REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.AnnouncementWithExpiresDateViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AdminAnnouncementViewData*/
    CLASS(
        UNSAFE, 'AdminAnnouncementViewData',
                EXTENDS(chlk.models.announcement.AnnouncementWithExpiresDateViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.announcementTypeName = 'Admin Announcement';
            }
        ]);
});
