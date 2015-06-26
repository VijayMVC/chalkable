REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AdminAnnouncementViewData*/
    CLASS(
        UNSAFE, 'AdminAnnouncementViewData',
                EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
            },

            chlk.models.common.ChlkDate, 'expiresDate'
        ]);
});
