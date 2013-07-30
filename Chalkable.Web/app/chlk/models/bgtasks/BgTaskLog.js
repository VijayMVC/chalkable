REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.BgTaskLogId');
NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            chlk.models.id.BgTaskLogId, 'id',
            //make enum
            [ria.serialize.SerializeProperty('logtype')],
            Number, 'logType',
            String, 'message',
            chlk.models.common.ChlkDate, 'added'
        ]);
});
