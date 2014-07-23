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
    CLASS(
        'Phone', [
            chlk.models.id.PhoneId, 'id',

            [ria.serialize.SerializeProperty('isprimary')],
            Boolean, 'isPrimary',

            Number, 'type',

            String, "value",


            Boolean, function isHomePhone(){
                return this.getType() == chlk.models.people.PhoneTypeEnum.Home.valueOf();
            }
        ]);
});
