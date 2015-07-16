REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            String, 'id',
            Number, 'level',
            String, 'message',
            [ria.serialize.SerializeProperty('timestamp')],
            chlk.models.common.ChlkDate, 'timeStamp'
        ]);
});
