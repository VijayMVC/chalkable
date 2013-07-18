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
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.storage.DbMaintenancePage, result);
            },

            [[chlk.models.storage.DatabaseUpdate]],
            function updateDbAction(model_){
                model_ = model_ || new chlk.models.storage.DatabaseUpdate;
                return this.PushView(chlk.activities.storage.DatabaseUpdatePage, new ria.async.DeferredData(model_));
            }

        ])
});
