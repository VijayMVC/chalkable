NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskType*/
    ENUM(
        'BgTaskType', {
            CREATE_EMPTY_SCHOOL: 0,
            SIS_DATA_IMPORT:1,
            BACKUP_DATABASES: 2,
            RESTORE_DATABASES: 3
        });
});
