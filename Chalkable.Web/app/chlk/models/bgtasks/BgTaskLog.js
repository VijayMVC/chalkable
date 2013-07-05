REQUIRE('chlk.models.common.ChlkDate');
NAMESPACE('chlk.models.bgtasks', function () {

    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            Number, 'id',
            //make enum
            [ria.serialize.SerializeProperty('logtype')],
            Number, 'logType',
            String, 'message',
            chlk.models.common.ChlkDate, 'added'
        ]);
});
