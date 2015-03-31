REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.bgtasks.BgTaskLog');
REQUIRE('chlk.models.id.BgTaskId');




NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.BgTaskService*/
    CLASS(
        'BgTaskService', EXTENDS(chlk.services.BaseService), [
            [[Number, Number, Number, chlk.models.id.DistrictId, Boolean]],
            ria.async.Future, function getTasks(pageIndex_, state_, type_, districtId_, allDistricts_) {
                return this.getPaginatedList('BackgroundTask/GetTasks.json', chlk.models.bgtasks.BgTask, {
                    start: pageIndex_,
                    state: state_,
                    type: type_,
                    districtId: districtId_ && districtId_.valueOf(),
                    allDistricts: allDistricts_
                });
            },
            [[chlk.models.id.BgTaskId, Number, Number]],
            ria.async.Future, function getLogs(id, start_, count_) {
                return this.getPaginatedList('BackgroundTask/GetTaskLogs.json', chlk.models.bgtasks.BgTaskLog, {
                    taskId: id.valueOf(),
                    start: start_,
                    count: count_
                });
            },
            [[chlk.models.id.BgTaskId]],
            ria.async.Future, function cancelTask(id) {
                return this.get('BackgroundTask/Cancel.json', null
                    , {
                            taskId: id.valueOf()
                      }
                );
            }
        ])
});