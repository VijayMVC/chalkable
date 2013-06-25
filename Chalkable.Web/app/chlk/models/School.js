NAMESPACE('chlk.models', function () {
    "use strict";
    /** @class chlk.models.School*/
    CLASS(
        'School', [
            Number, 'id',
            String, 'name',
            Number, 'localid',
            Number, 'ncesid',
            String, 'schooltype',
            String, 'schoolurl',
            Boolean,'sendemailnotifications',
            String, 'timezoneid'
        ]);
});
