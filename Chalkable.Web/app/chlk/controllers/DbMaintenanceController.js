REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DbMaintenanceService');
REQUIRE('chlk.activities.storage.DbMaintenancePage');
REQUIRE('chlk.activities.storage.DatabaseUpdatePage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DbMaintenanceController */
    CLASS(
        'DbMaintenanceController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.DbMaintenanceService, 'dbMaintenanceService',

            [[Number]],
            function listBackupsAction(start_){
                var result = this.dbMaintenanceService
                    .getBackups(start_ || 0, 10)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        this.getContext().getSession().set('listBackups', model);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.storage.DbMaintenancePage, result);
            },

            [[chlk.models.storage.DatabaseUpdate]],
            function updateDbAction(model_){
                model_ = model_ || new chlk.models.storage.DatabaseUpdate();
                return this.PushView(chlk.activities.storage.DatabaseUpdatePage, new ria.async.DeferredData(model_));
            },

            [[chlk.models.storage.DatabaseUpdate]],
            function runSqlAction(model){
                this.dbMaintenanceService
                    .databaseUpdate(
                        model.getMasterSql(),
                        model.getSchoolSql()
                    )
                    .attach(this.validateResponse_());
                this.ShowMsgBox('Db update task is started.', 'fyi.', [{
                    text: Msg.GOT_IT.toUpperCase(),
                    controller: "settings",
                    action: "dashboard"
                }]);
            },

            function backupAction(){
                var result = this.dbMaintenanceService
                    .backup()
                    .then(function(success){
                        return this.getContext().getSession().get('listBackups');
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.storage.DbMaintenancePage, result);
            },

            [[String]],
            function restoreAction(ticks){
                var result = this.dbMaintenanceService
                    .restore(ticks)
                    .then(function(success){
                        return this.getContext().getSession().get('listBackups');
                    }, this);
                return this.UpdateView(chlk.activities.storage.DbMaintenancePage, result);
            }
        ])
});
