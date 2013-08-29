REQUIRE('chlk.models.id.RoomId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.Room*/
    CLASS(
        'Room', [
            Object, 'building',

            Number, 'capacity',

            String, 'description',

            [ria.serialize.SerializeProperty('homeroomnumber')],
            Number, 'homeRoomNumber',

            chlk.models.id.RoomId, 'id',

            [ria.serialize.SerializeProperty('phonenumber')],
            Number, 'phoneNumber',

            [ria.serialize.SerializeProperty('roomnumber')],
            Number, 'roomNumber',

            [ria.serialize.SerializeProperty('roomtypedescription')],
            Number, 'roomTypeDescription',

            [ria.serialize.SerializeProperty('roomtypeid')],
            Number, 'roomTypeId',

            Object, 'size'
        ]);
});
