REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.ShortUserInfo*/
    CLASS(
        'ShortUserInfo', [

            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',

            String, 'email',

            [ria.serialize.SerializeProperty('firstname')],
            String, 'firstName',

            [ria.serialize.SerializeProperty('fullname')],
            String, 'fullName',

            String, 'gender',

            chlk.models.id.SchoolPersonId, 'id',

            [ria.serialize.SerializeProperty('lastname')],
            String, 'lastName',

            String, 'password',

            String, 'pictureUrl',

            String, 'roleName',

            [ria.serialize.SerializeProperty('role')],
            chlk.models.people.Role, 'role',

            String, 'genderFullText',

            String, 'salutation'

        ]);
});
