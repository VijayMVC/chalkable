REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.RoomId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.PersonSummary*/
    CLASS(
        'PersonSummary', EXTENDS(chlk.models.people.User), IMPLEMENTS(ria.serialize.IDeserializable),[

            chlk.models.id.RoomId, 'roomId',
            String, 'roomName',
            Number, 'roomNumber',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.roomId = SJX.fromValue(raw.roomid, chlk.models.id.RoomId);
                this.roomName = SJX.fromValue(raw.roomname, String);
                this.roomNumber = SJX.fromValue(raw.roomnumber, Number);
            }
    ]);
});