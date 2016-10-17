REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.models.id.BgTaskId');
REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.bgtasks.BgTask');
REQUIRE('chlk.models.bgtasks.BgTasksLogListViewData');
REQUIRE('chlk.models.bgtasks.BgTasksListViewData');
REQUIRE('chlk.models.bgtasks.BgTaskState');
REQUIRE('chlk.models.bgtasks.BgTaskType');
REQUIRE('chlk.models.bgtasks.GetBgTasksPostData');
REQUIRE('chlk.models.bgtasks.RerunTasksPostData');

REQUIRE('chlk.activities.bgtasks.BgTasksListPage');
REQUIRE('chlk.activities.bgtasks.BgTaskLogListPage');

REQUIRE('chlk.services.BgTaskService');
REQUIRE('chlk.services.DistrictService');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.BackgroundTaskController */
    CLASS(
        'BackgroundTaskController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.BgTaskService, 'bgTaskService',

            [ria.mvc.Inject],
            chlk.services.DistrictService, 'districtService',

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.bgtasks.GetBgTasksPostData]],
            function listAction(postData_) {
                if(postData_)
                    return this.getTasks_(
                        postData_.getStart(),
                        postData_.getState(),
                        postData_.getType(),
                        postData_.getDistrictId(),
                        postData_.isAllDistricts()
                    )
                return this.getTasks_(0, null, null, null, true);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[Number, Number, chlk.models.id.DistrictId, Boolean, Number]],
            function pageAction(state_, type_, districtId_, allDistricts_, start_) {
                return this.getTasks_(start_, state_, type_, districtId_, allDistricts_);
            },

            [[Number, Number, Number, chlk.models.id.DistrictId, Boolean]],
            function getTasks_(pageIndex_, state_, type_, districtId_, allDistricts_){
                var result =
                    ria.async.wait(
                        this.bgTaskService.getTasks(pageIndex_ | 0, 50, state_, type_, districtId_, allDistricts_),
                        this.districtService.getDistricts(0)
                    )
                    .attach(this.validateResponse_())
                    .then(function(res){
                            var states = [
                                new chlk.models.bgtasks.BgTaskState(chlk.models.bgtasks.BgTaskStateEnum.CREATED),
                                new chlk.models.bgtasks.BgTaskState(chlk.models.bgtasks.BgTaskStateEnum.PROCESSING),
                                new chlk.models.bgtasks.BgTaskState(chlk.models.bgtasks.BgTaskStateEnum.PROCESSED),
                                new chlk.models.bgtasks.BgTaskState(chlk.models.bgtasks.BgTaskStateEnum.CANCELED),
                                new chlk.models.bgtasks.BgTaskState(chlk.models.bgtasks.BgTaskStateEnum.FAILED),
                            ];
                            var types = [
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.CREATE_EMPTY_SCHOOL),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.SIS_DATA_IMPORT),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.BACKUP_DATABASES),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.RESTORE_DATABASES),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.DATABASE_UPDATE),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.CREATE_DEMO_SCHOOL),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.DELETE_SCHOOL),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.PROCESS_REMINDERS),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.ATTENDANCE_NOTIFICATION),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.TEACHER_ATTENDANCE_NOTIFICATION),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.PICTURE_IMPORT),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.RE_SYNC),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.DACPAC_UPDATE),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.AB_IMPORT),
                                new chlk.models.bgtasks.BgTaskType(chlk.models.bgtasks.BgTaskTypeEnum.REPORT_PROCESSING),

                            ];
                            return new chlk.models.bgtasks.BgTasksListViewData(res[0], res[1].getItems(), states, types, state_, type_, districtId_, allDistricts_);
                    });
                return this.PushOrUpdateView(chlk.activities.bgtasks.BgTasksListPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.id.BgTaskId, Boolean, Number]],
            function logsPageAction(id, postback_, start_) {
                var result = this.bgTaskService
                    .getLogs(id, start_ || 0, 100)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return new ria.async.DeferredData(new chlk.models.bgtasks.BgTasksLogListViewData(id, data));
                    });
                return postback_
                    ? this.UpdateView(chlk.activities.bgtasks.BgTaskLogListPage, result)
                    : this.PushView(chlk.activities.bgtasks.BgTaskLogListPage, result);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.id.BgTaskId]],
            ria.async.Future, function tryCancelAction(id){
                var msgText = "Do you realy want to Cancel this task?";
                return this.ShowConfirmBox(msgText, "whoa.", null, 'negative-button')
                    .thenCall(this.bgTaskService.cancelTask, [id])
                    .attach(this.validateResponse_())
                    .then(function() {
                        return this.Redirect('backgroundtask', 'list', [])
                    }, this);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.id.BgTaskId]],
            ria.async.Future, function tryRerunAction(id){
                var msgText = "Do you realy want to Rerun this task?";
                return this.ShowConfirmBox(msgText, "whoa.", 'RERUN', 'negative-button')
                    .thenCall(this.bgTaskService.rerunTask, [id])
                    .attach(this.validateResponse_())
                    .then(function() {
                        return this.Redirect('backgroundtask', 'list', [])
                    }, this);
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.SYSADMIN
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.bgtasks.RerunTasksPostData]],
            ria.async.Future, function tryRerunAllAction(postData){
                var msgText = "Do you realy want to Rerun All tasks?";
                return this.ShowConfirmBox(msgText, "whoa.", 'RERUN', 'negative-button')
                    .thenCall(this.bgTaskService.rerunTasks, [ this.getIdsList(postData.getTasksIdsStr(), chlk.models.id.BgTaskId)])
                    .attach(this.validateResponse_())
                    .then(function() {
                        return this.Redirect('backgroundtask', 'list', [])
                    }, this);
            }
        ])
});
