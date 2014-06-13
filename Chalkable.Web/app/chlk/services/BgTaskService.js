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
            [[Number, Number]],
            ria.async.Future, function getTasks(pageIndex_, state_) {
                return this.getPaginatedList('BackgroundTask/GetTasks.json', chlk.models.bgtasks.BgTask, {
                    start: pageIndex_,
                    state: state_
                });
            },
            [[chlk.models.id.BgTaskId, Number]],
            ria.async.Future, function getLogs(id, start_) {
                return this.getPaginatedList('BackgroundTask/GetTaskLogs.json', chlk.models.bgtasks.BgTaskLog, {
                    taskId: id.valueOf(),
                    start: start_
                });
            },
            [[chlk.models.id.BgTaskId]],
            ria.async.Future, function deleteTask(id) {
                return this.get('BackgroundTask/Delete.json', null
                    , {
                            taskId: id.valueOf()
                      }
                );
            }
        ])
});