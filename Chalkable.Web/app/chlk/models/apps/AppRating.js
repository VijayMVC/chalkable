REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";




    /** @class chlk.models.apps.PersonRating*/
    CLASS(
        'PersonRating', [
            chlk.models.people.ShortUserInfo, 'person',
            Number, 'rating',
            String, 'review'
    ]);

    /** @class chlk.models.apps.RoleRating*/
    CLASS(
        'RoleRating', [

            [ria.serialize.SerializeProperty('avgrating')],
            Number, 'rating',

            [ria.serialize.SerializeProperty('personcount')],
            Number, 'personCount',

            chlk.models.people.Role, 'role'


    ]);



    /** @class chlk.models.apps.AppRating*/
    CLASS(
        'AppRating', [
            [ria.serialize.SerializeProperty('avg')],
            Number, 'average',

            [ria.serialize.SerializeProperty('ratingbyperson')],
            ArrayOf(chlk.models.apps.PersonRating), 'personRatings',

            [ria.serialize.SerializeProperty('ratingbyroles')],
            ArrayOf(chlk.models.apps.RoleRating), 'roleRatings'
        ]);
});
