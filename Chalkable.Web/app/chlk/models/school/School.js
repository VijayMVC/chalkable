REQUIRE('chlk.models.school.Timezone');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.School*/
    CLASS(
        'School', [
            Number, 'id',
            String, 'name',
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
            [ria.serialize.SerializeProperty('timezoneid')],
            String, 'timezoneId',
            ArrayOf(chlk.models.school.Timezone), 'timezones',
            Number, 'districtId'
        ]);
});
