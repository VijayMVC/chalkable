REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.DeveloperInfo*/
    CLASS(
        'DeveloperInfo', [
            chlk.models.id.SchoolPersonId, 'id',
            [ria.serialize.SerializeProperty('displayname')],
            String, 'displayName',
            String, 'email',
            [ria.serialize.SerializeProperty('firstname')],
            String, 'firstName',
            [ria.serialize.SerializeProperty('lastname')],
            String, 'lastName',
            String, 'name',
            [ria.serialize.SerializeProperty('schoolid')],
            chlk.models.id.SchoolId, 'schoolId',
            [ria.serialize.SerializeProperty('websitelink')],
            String, 'webSite',

            [ria.serialize.SerializeProperty('paypallogin')],
            String, 'payPalAddress'
        ]);
});
