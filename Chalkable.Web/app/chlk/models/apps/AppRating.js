REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.apps.AppPersonRating');
REQUIRE('chlk.models.apps.AppRoleRating');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.apps.AppRating*/
    CLASS(
        UNSAFE, FINAL, 'AppRating', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.average = SJX.fromValue(raw.avg, Number);
                this.personRatings = SJX.fromArrayOfDeserializables(raw.ratingbyperson, chlk.models.apps.AppPersonRating);
                this.roleRatings = SJX.fromArrayOfDeserializables(raw.ratingbyroles, chlk.models.apps.AppRoleRating);
            },

            Number, 'average',
            ArrayOf(chlk.models.apps.AppPersonRating), 'personRatings',
            ArrayOf(chlk.models.apps.AppRoleRating), 'roleRatings'
        ]);
});
