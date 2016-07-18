REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.models.bgtasks', function () {
    "use strict";

    /** @class chlk.models.bgtasks.RerunTasksPostData*/
    CLASS(
        'RerunTasksPostData', [
            [ria.serialize.SerializeProperty('tasksids')],
            String, 'tasksIdsStr',
        ]);
});
