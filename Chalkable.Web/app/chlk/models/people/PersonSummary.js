REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.RoomId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.PersonSummary*/
    CLASS(
        'PersonSummary', EXTENDS(chlk.models.people.User),[

            [ria.serialize.SerializeProperty('roomid')],
            chlk.models.id.RoomId, 'roomId',

            [ria.serialize.SerializeProperty('roomname')],
            String, 'roomName',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber'
    ]);
});