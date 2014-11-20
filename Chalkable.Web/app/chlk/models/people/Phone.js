REQUIRE('chlk.models.id.PhoneId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.PhoneTypeEnum*/
    ENUM('PhoneTypeEnum',{
        Home: 0,
        Work: 1,
        Mobile: 2
    });


    /** @class chlk.models.people.Phone*/
    CLASS(FINAL, UNSAFE, 'Phone', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw){
           this.id = SJX.fromValue(data.id, chlk.models.id.PhoneId);
           this.isPrimary = SJX.fromValue(data.isprimary, Boolean);
           this.type = SJX.fromValue(data.type, Number);
           this.value = SJX.fromValue(data.value, String);
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
