REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";




    /** @class chlk.models.apps.PersonRating*/
    CLASS(
        'PersonRating', [
            [ria.serialize.SerializeProperty('firstname')],
            String, 'firstName',

            [ria.serialize.SerializeProperty('lastname')],
            String, 'lastName',

            Number, 'rating',
            String, 'review',

            [ria.serialize.SerializeProperty('roleid')],
            Number, 'roleId',

            [ria.serialize.SerializeProperty('rolename')],
            String, 'roleName',

            [ria.serialize.SerializeProperty('schoolpersonid')],
            chlk.models.id.SchoolPersonId, 'schoolPersonId'
    ]);

    /** @class chlk.models.apps.RoleRating*/
    CLASS(
        'RoleRating', [

            [ria.serialize.SerializeProperty('avgrating')],
            Number, 'rating',

            [ria.serialize.SerializeProperty('personcount')],
            Number, 'personCount',

            [ria.serialize.SerializeProperty('roleid')],
            Number, 'roleId',

            [ria.serialize.SerializeProperty('rolename')],
            String, 'roleName'
    ]);



    /** @class chlk.models.apps.AppRating*/
    CLASS(
        'AppRating', [
            [ria.serialize.SerializeProperty('avg')],
            Number, 'average',

            [ria.serialize.SerializeProperty('ratingbyperson')],
            ArrayOf(chlk.models.apps.PersonRating), 'personRatings',

            [ria.serialize.SerializeProperty('ratingbyrole')],
            ArrayOf(chlk.models.apps.RoleRating), 'roleRatings'
        ]);
});
