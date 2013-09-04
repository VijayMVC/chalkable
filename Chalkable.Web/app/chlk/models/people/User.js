REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Address');
REQUIRE('chlk.models.people.Phone');
REQUIRE('chlk.models.people.Role');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.User*/
    CLASS(
        'User', [
            Boolean, 'active',

            ArrayOf(chlk.models.people.Address), 'addresses',

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

            String, 'password',

            String, 'pictureUrl',

            String, 'roleName',

            [ria.serialize.SerializeProperty('role')],
            chlk.models.people.Role, 'role',

            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'schoolId',

            [ria.serialize.SerializeProperty('birthdate')],
            chlk.models.common.ChlkDate, 'birthDate',

            String, 'birthDateText',

            String, 'genderFullText',

            ArrayOf(chlk.models.people.Phone), 'phones',

            String, 'salutation',

            Boolean, 'ableEdit',

            chlk.models.people.Phone, 'primaryPhone',

            chlk.models.people.Phone, 'homePhone',

            String, 'addressesValue',

            String, 'phonesValue',

            Number, 'index',

            Boolean, 'selected'
        ]);
});
