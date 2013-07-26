REQUIRE('chlk.models.bgtasks.BgTaskState');
REQUIRE('chlk.models.bgtasks.BgTaskType');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.bgtasks', function () {

    "use strict";
    /** @class chlk.models.bgtasks.BgTask*/
    CLASS(
        'BgTask', [
            chlk.models.id.BgTaskId, 'id',
            chlk.models.common.ChlkDate, 'created',
            chlk.models.common.ChlkDate, 'scheduled',
            chlk.models.common.ChlkDate, 'completed',
            [ria.serialize.SerializeProperty('taskstate')],
            chlk.models.bgtasks.BgTaskState, 'taskState',
            [ria.serialize.SerializeProperty('tasktype')],
            chlk.models.bgtasks.BgTaskType, 'taskType'
        ]);
});
