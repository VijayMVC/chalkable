NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolSisInfo*/
    CLASS(
        'SchoolSisInfo', [
            Number, 'attendanceSyncFreq',
            Number, 'disciplineSyncFreq',
            Number, 'personSyncFreq',
            Number, 'scheduleSyncFreq',
            Number, 'id',
            String, 'sisUrl',
            String, 'sisUserName',
            String, 'sisPassword',
            String, 'sisName'
        ]);
});
