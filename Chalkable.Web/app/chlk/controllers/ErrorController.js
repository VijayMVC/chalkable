REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.chlkerror.Error404Page');
REQUIRE('chlk.models.apps.AppErrorViewData');
REQUIRE('chlk.models.common.ServerErrorModel');
REQUIRE('chlk.activities.chlkerror.AppErrorDialog');
REQUIRE('chlk.activities.common.PermissionsErrorPage');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.chlkerror.GeneralServerErrorPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ErrorController*/
    CLASS(
        'ErrorController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            function error404Action() {
                var result = new ria.async.DeferredData(new chlk.models.Success());
                return this.PushView(chlk.activities.chlkerror.Error404Page, result);
            },

            [[String]],
            function generalServerErrorAction(message){
                var res = new ria.async.DeferredData(new chlk.models.common.ServerErrorModel(message));
                return this.PushView(chlk.activities.chlkerror.GeneralServerErrorPage, res);
            },

            function appErrorAction(){
                var result = new ria.async.DeferredData(new chlk.models.apps.AppErrorViewData('someemail'));
                return this.ShadeView(chlk.activities.chlkerror.AppErrorDialog, result);
            },

            [[ArrayOf(chlk.models.people.UserPermissionEnum)]],
            function permissionsAction(permissions){
                var topModel = new chlk.models.classes.ClassesForTopBar(this.classService.getClassesForTopBar(true));
                var result = new ria.async.DeferredData(new chlk.models.common.PermissionsError(topModel, null, permissions));
                return this.PushView(chlk.activities.common.PermissionsErrorPage, result);
            }
    ])
});
