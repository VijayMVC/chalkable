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
            //date timestamp
            [ria.serialize.SerializeProperty('schoolname')],
            String, 'schoolName',
            [ria.serialize.SerializeProperty('localid')],
            Number, 'localId',
            [ria.serialize.SerializeProperty('ncesid')],
            Number, 'ncesId',
            [ria.serialize.SerializeProperty('schooltype')],
            String, 'schoolType',
            [ria.serialize.SerializeProperty('schoolurl')],
            String, 'schoolUrl',
            [ria.serialize.SerializeProperty('sendemailnotofications')],
            Boolean,'sendEmailNotifications',
            [ria.serialize.SerializeProperty('timezone')],
            String, 'timezoneId',
        ]);
});