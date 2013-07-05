NAMESPACE('chlk.models.bgtasks', function () {

    "use strict";
    /** @class chlk.models.bgtasks.BgTaskLog*/
    CLASS(
        'BgTaskLog', [
            Number, 'id',
            //make enum
            [ria.serialize.SerializeProperty('logtype')],
            Number, 'logType',
            String, 'message'
            //Date, 'added',
        ]);
});
