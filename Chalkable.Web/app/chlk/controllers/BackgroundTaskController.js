REQUIRE('chlk.controllers.BaseController');

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
                /* Put activity in stack and render when result is ready */
                return this.PushView(chlk.activities.bgtasks.BgTasksListPage, result);
            },
            [[Number]],
            function logsAction(id) {
                var result = this.bgTaskService
                    .getLogs(id)
                    .attach(this.validateResponse_());
                /* Put activity in stack and render when result is ready */
                return this.PushView(chlk.activities.bgtasks.BgTaskLogListPage, result);
            }
        ])
});
