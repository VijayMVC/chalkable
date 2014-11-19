REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.announcement.AnnouncementMessage*/
    CLASS(
        UNSAFE, FINAL, 'AnnouncementMessage', IMPLEMENTS(ria.serialize.IDeserializable),  [

            VOID, function deserialize(raw){
                this.created = SJX.fromDeserializable(raw.created, chlk.models.common.ChlkDate);
                this.person = SJX.fromDeserializable(raw.person, chlk.models.people.User);
                this.message = SJX.fromValue(raw.message, String);
            },

            chlk.models.common.ChlkDate, 'created',
            chlk.models.people.User, 'person',
            String, 'message'
        ]);
});
