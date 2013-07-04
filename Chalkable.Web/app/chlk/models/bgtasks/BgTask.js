NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";
    /** @class chlk.models.bgtasks,BgTask*/
    CLASS(
        'BgTask', [
            Number, 'id',
            //Date, 'created',
            //Date, 'scheduled',
            //Date, 'completed',
            [ria.serialize.SerializeProperty('taskstate')],
            Number, 'taskState',
            [ria.serialize.SerializeProperty('tasktype')],
            Number, 'taskType'
        ]);
});
