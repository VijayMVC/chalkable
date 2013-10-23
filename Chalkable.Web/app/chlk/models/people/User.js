REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.Address');
REQUIRE('chlk.models.people.Phone');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.User*/
    CLASS(
        'User', EXTENDS(chlk.models.people.ShortUserInfo), [
            Boolean, 'active',

            ArrayOf(chlk.models.people.Address), 'addresses',

            String, 'grade',

            [ria.serialize.SerializeProperty('localid')],
            String, 'localId',

            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'schoolId',

            [ria.serialize.SerializeProperty('birthdate')],
            chlk.models.common.ChlkDate, 'birthDate',

            String, 'birthDateText',

            ArrayOf(chlk.models.people.Phone), 'phones',

            Boolean, 'ableEdit',

            chlk.models.people.Phone, 'primaryPhone',

            chlk.models.people.Phone, 'homePhone',

            String, 'addressesValue',

            String, 'phonesValue',

            Number, 'index',

            Boolean, 'selected',

            [[String, String, chlk.models.id.SchoolPersonId]],
            function $(firstName_, lastName_, id_){
                BASE(firstName_, lastName_, id_);
            }
        ]);
});
