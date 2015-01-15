REQUIRE('chlk.models.id.AddressId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.Address*/
    CLASS(UNSAFE, FINAL, 'Address', IMPLEMENTS(ria.serialize.IDeserializable),  [

        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.AddressId);
            this.type = SJX.fromValue(raw.type, Number);
            this.value = SJX.fromValue(raw.value, String);
        },

        chlk.models.id.AddressId, 'id',
        Number, 'type',
        String, "value"
    ]);
});
