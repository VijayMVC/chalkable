REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.UserForLunchViewData*/
    CLASS(
        UNSAFE, 'UserForLunchViewData', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),  [

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw);
            this.absent = SJX.fromValue(raw.isabsent, Boolean);
        },

        Boolean, 'absent'
    ]);
});
