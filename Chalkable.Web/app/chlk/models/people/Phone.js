REQUIRE('chlk.models.id.PhoneId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.PhoneTypeEnum*/
    ENUM('PhoneTypeEnum',{
        Home: 0,
        Work: 1,
        Mobile: 2
    });


    /** @class chlk.models.people.Phone*/
    CLASS(FINAL, UNSAFE, 'Phone', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw){
           this.id = SJX.fromValue(raw.id, chlk.models.id.PhoneId);
           this.isPrimary = SJX.fromValue(raw.isprimary, Boolean);
           this.type = SJX.fromValue(raw.type, Number);
           this.value = SJX.fromValue(raw.value, String);
        },

        chlk.models.id.PhoneId, 'id',
        Boolean, 'isPrimary',
        Number, 'type',
        String, "value",

        Boolean, function isHomePhone(){
            return this.getType() == chlk.models.people.PhoneTypeEnum.Home.valueOf();
        }
    ]);
});
