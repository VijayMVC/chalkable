REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.AppPersonRating*/
    CLASS(
        UNSAFE, FINAL, 'AppPersonRating',  IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.person = SJX.fromDeserializable(raw.person, chlk.models.people.ShortUserInfo);
                this.rating = SJX.fromValue(raw.rating, Number);
                this.review = SJX.fromValue(raw.review, String);
            },
            chlk.models.people.ShortUserInfo, 'person',
            Number, 'rating',
            String, 'review'
        ]);
});
