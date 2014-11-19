REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.AppRoleRating*/
    CLASS(
        UNSAFE, FINAL, 'AppRoleRating',  IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.rating = SJX.fromValue(raw.avgrating, Number);
                this.personCount = SJX.fromValue(raw.personcount, Number);
                this.role = SJX.fromDeserializable(raw.role, chlk.models.people.Role);
            },
            Number, 'rating',
            Number, 'personCount',
            chlk.models.people.Role, 'role'
        ]);
});
