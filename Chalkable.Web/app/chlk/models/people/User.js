REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.User*/
    CLASS(
        'User', [
            Boolean, 'active',

            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',

            String, 'email',

            [ria.serialize.SerializeProperty('firstname')],
            String, 'firstName',

            [ria.serialize.SerializeProperty('fullname')],
            String, 'fullName',

            String, 'gender',

            String, 'grade',

            chlk.models.id.SchoolPersonId, 'id',

            [ria.serialize.SerializeProperty('lastname')],
            String, 'lastName',

            [ria.serialize.SerializeProperty('localid')],
            String, 'localId',

            [ria.serialize.SerializeProperty('roledescription')],
            String, 'roleDescription',

            [ria.serialize.SerializeProperty('rolename')],
            String, 'roleName',

            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'schoolId'
        ]);
});
