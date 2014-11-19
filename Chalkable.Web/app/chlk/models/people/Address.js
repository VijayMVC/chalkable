REQUIRE('chlk.models.id.AddressId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.Address*/
    CLASS(UNSAFE, FINAL, 'Address', IMPLEMENTS(ria.serialize.IDeserializable),  [

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(data.id, chlk.models.id.AddressId);
            this.type = SJX.fromValue(data.type, Number);
            this.value = SJX.fromValue(data.value, String);
        },

        chlk.models.id.AddressId, 'id',
        Number, 'type',
        String, "value"
    ]);
});
