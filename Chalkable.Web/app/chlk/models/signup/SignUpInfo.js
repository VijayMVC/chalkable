REQUIRE('chlk.models.common.ChlkDate');
NAMESPACE('chlk.models.signup', function () {
    "use strict";
    /** @class chlk.models.signup.SignUpInfo*/
    CLASS(
        'SignUpInfo', [
            Number, 'id',
            [ria.serialize.SerializeProperty('schoolid')],
            Number, 'schoolId',
            [ria.serialize.SerializeProperty('usertype')],
            String, 'userType',
            String, 'email',
            String, 'password',
            String, 'name',
            [ria.serialize.SerializeProperty('schoolname')],
            String, 'schoolName',
            [ria.serialize.SerializeProperty('ipaddress')],
            String, 'ipAddress',
            [ria.serialize.SerializeProperty('sisurl')],
            String, 'sisUrl',
            [ria.serialize.SerializeProperty('sisusername')],
            String, 'sisUserName',
            [ria.serialize.SerializeProperty('sispassword')],
            String, 'sisPassword',
            [ria.serialize.SerializeProperty('systemtype')],
            String, 'systemType',
            [ria.serialize.SerializeProperty('candelete')],
            String, 'canDelete',
            [ria.serialize.SerializeProperty('timestamp')],
            chlk.models.common.ChlkDate, 'timeStamp',
            [ria.serialize.SerializeProperty('timezone')],
            String, 'timezone'
        ]);
});