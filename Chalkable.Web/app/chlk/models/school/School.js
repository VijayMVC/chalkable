REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.School*/
    CLASS(
        'School', [
            chlk.models.id.SchoolId, 'id',
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
            [ria.serialize.SerializeProperty('districtid')],
            chlk.models.id.DistrictId, 'districtId'
        ]);
});
