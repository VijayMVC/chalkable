REQUIRE('chlk.models.id.RoomId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.Room*/
    CLASS(
        'Room', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.building = SJX.fromValue(raw.building, Object);
                this.capacity = SJX.fromValue(raw.capacity, Number);
                this.description = SJX.fromValue(raw.description, String);
                this.homeRoomNumber = SJX.fromValue(raw.homeroomnumber, Number);
                this.id = SJX.fromValue(raw.id, chlk.models.id.RoomId);
                this.phoneNumber = SJX.fromValue(raw.phonenumber, Number);
                this.roomNumber = SJX.fromValue(raw.roomnumber, String);
                this.roomTypeDescription = SJX.fromValue(raw.roomtypedescription, Number);
                this.roomTypeId = SJX.fromValue(raw.roomtypeid, Number);
                this.size = raw.size == null ? null : raw.size;
            },

            Object, 'building',

            Number, 'capacity',

            String, 'description',

            Number, 'homeRoomNumber',

            chlk.models.id.RoomId, 'id',

            Number, 'phoneNumber',

            String, 'roomNumber',

            Number, 'roomTypeDescription',

            Number, 'roomTypeId',

            Object, 'size'
        ]);
});
