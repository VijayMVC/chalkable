REQUIRE('chlk.models.common.ChlkDate');
NAMESPACE('chlk.models.bgtasks', function () {

    /** @class chlk.models.bgtasks.BgTaskLogId*/
    IDENTIFIER('BgTaskLogId');

    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            chlk.models.bgtasks.BgTaskLogId, 'id',
            //make enum
            [ria.serialize.SerializeProperty('logtype')],
            Number, 'logType',
            String, 'message',
            chlk.models.common.ChlkDate, 'added'
        ]);
});
