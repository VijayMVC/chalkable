REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.bgtasks.BgTaskLog');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.BgTaskService*/
    CLASS(
        'BgTaskService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getTasks(pageIndex_) {
                return this.getPaginatedList('BackgroundTask/GetTasks.json', chlk.models.bgtasks.BgTask, {
                    start: pageIndex_
                });
            },
            [[chlk.models.bgtasks.BgTaskId, Number]],
            ria.async.Future, function getLogs(id, pageIndex_) {
                return this.getPaginatedList('BackgroundTask/GetTaskLogs.json', chlk.models.bgtasks.BgTaskLog, {
                    taskId: id.valueOf(),
                    start: pageIndex_
                });
            }
        ])
});