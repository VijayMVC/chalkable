REQUIRE('chlk.models.id.SchoolSisInfoId');
NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolSisInfo*/
    CLASS(
        'SchoolSisInfo', [
            Number, 'attendanceSyncFreq',
            Number, 'disciplineSyncFreq',
            Number, 'personSyncFreq',
            Number, 'scheduleSyncFreq',
            chlk.models.id.SchoolSisInfoId, 'id',
            String, 'sisUrl',
            String, 'sisUserName',
            String, 'sisPassword',
            String, 'sisName'
        ]);
});
