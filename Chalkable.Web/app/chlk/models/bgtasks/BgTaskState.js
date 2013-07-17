NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskState*/
    ENUM(
        'BgTaskState', {
            CREATED: 0,
            PROCESSING: 1,
            PROCESSED: 2,
            CANCELED: 3,
            FAILED: 4
        });
});
