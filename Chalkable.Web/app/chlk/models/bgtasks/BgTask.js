REQUIRE('chlk.models.bgtasks.BgTaskState');
REQUIRE('chlk.models.bgtasks.BgTaskType');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.BgTaskId');

NAMESPACE('chlk.models.bgtasks', function () {

    "use strict";
    /** @class chlk.models.bgtasks.BgTask*/
    CLASS(
        'BgTask', [
            chlk.models.id.BgTaskId, 'id',
            chlk.models.common.ChlkDate, 'created',
            chlk.models.common.ChlkDate, 'scheduled',
            chlk.models.common.ChlkDate, 'completed',
            chlk.models.common.ChlkDate, 'started',
            [ria.serialize.SerializeProperty('taskstate')],
            chlk.models.bgtasks.BgTaskState, 'taskState',
            [ria.serialize.SerializeProperty('tasktype')],
            chlk.models.bgtasks.BgTaskType, 'taskType',
            [ria.serialize.SerializeProperty('schoolid')],
            chlk.models.id.SchoolId, 'schoolId'
        ]);
});
