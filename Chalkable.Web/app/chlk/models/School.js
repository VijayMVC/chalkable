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
            String, 'timezoneId'
        ]);
});
