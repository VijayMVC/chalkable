REQUIRE('chlk.models.bgtasks.BgTaskState');
REQUIRE('chlk.models.bgtasks.BgTaskType');

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
            chlk.models.bgtasks.BgTaskState, 'taskState',
            [ria.serialize.SerializeProperty('tasktype')],
            chlk.models.bgtasks.BgTaskType, 'taskType'
        ]);
});
