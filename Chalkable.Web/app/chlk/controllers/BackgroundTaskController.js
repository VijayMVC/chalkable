REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.services.BgTaskService');
REQUIRE('chlk.activities.bgtasks.BgTasksListPage');
REQUIRE('chlk.activities.bgtasks.BgTaskLogListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.BackgroundTaskController */
    CLASS(
        'BackgroundTaskController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.BgTaskService, 'bgTaskService',

            [chlk.controllers.SidebarButton('apps')],
            [[Number]],
            function listAction(pageIndex_) {
                var result = this.bgTaskService
                    .getTasks(pageIndex_ | 0)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.bgtasks.BgTasksListPage, result);
            },


            [[Number]],
            function pageAction(pageIndex_) {
                var result = this.bgTaskService
                    .getTasks(pageIndex_ | 0)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.bgtasks.BgTasksListPage, result);
            },

            [[chlk.models.bgtasks.BgTaskId]],
            function logsAction(id) {
                var result = this.bgTaskService
                    .getLogs(id)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.bgtasks.BgTaskLogListPage, result);
            },

            [[chlk.models.bgtasks.BgTaskId, Number]],
            function logsPageAction(id, pageIndex_) {
                var result = this.bgTaskService
                    .getLogs(id, pageIndex)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.bgtasks.BgTaskLogListPage, result);
            },
            [[Number]],
            function deleteAction(id){

            }
        ])
});
