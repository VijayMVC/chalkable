REQUIRE('chlk.models.school.Timezone');

NAMESPACE('chlk.models', function () {
    "use strict";
    /** @class chlk.models.School*/
    CLASS(
        'School', [
            Number, 'id',
            String, 'name',
            Number, 'localId',
            Number, 'ncesId',
            String, 'schoolType',
            String, 'schoolUrl',
            Boolean,'sendEmailNotifications',
            [ria.serialize.SerializeProperty('timezoneid')],
            String, 'timezoneId',
            ArrayOf(chlk.models.school.Timezone), 'timezones'
        ]);
});
