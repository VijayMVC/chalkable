REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.chlkerror.Error404Page');
REQUIRE('chlk.models.apps.AppWrapperViewData');
REQUIRE('chlk.models.common.ServerErrorModel');
REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.activities.common.PermissionsErrorPage');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.chlkerror.GeneralServerErrorPage');
REQUIRE('chlk.activities.chlkerror.GeneralServerErrorWithClassesPage');

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

            function createAnnouncementErrorAction(){
                var message = 'You have to set up category types in iNow before you can send a new item in Chalkable.';
                return this.generalServerErrorWithClassesAction(message, false, 'announcement', 'add');
            },

            function viewAnnouncementErrorAction(){
                var message = 'This item has been deleted or you don\'t have access to view it';
                return this.generalServerErrorAction(message);
            },

            [[String]],
            function generalServerErrorWithClassesAction(message, withAll_, controller_, action_, params_){
                var topModel = new chlk.models.classes.ClassesForTopBar(this.classService.getClassesForTopBarSync(withAll_));
                var res = new ria.async.DeferredData(new chlk.models.common.ServerErrorWithClassesModel.$create(topModel, message, controller_, action_, params_));
                return this.PushView(chlk.activities.chlkerror.GeneralServerErrorWithClassesPage, res);
            },

            [[String]],
            function generalServerErrorAction(message){
                var res = new ria.async.DeferredData(new chlk.models.common.ServerErrorModel(message));
                return this.PushView(chlk.activities.chlkerror.GeneralServerErrorPage, res);
            },

            function appErrorAction(){
                var result = new ria.async.DeferredData(chlk.models.apps.AppWrapperViewData.$createAppErrorViewData(''));
                return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
            },

            [[Array]],
            function permissionsAction(permissions){
                var topModel = new chlk.models.classes.ClassesForTopBar(null);
                var result = new ria.async.DeferredData(new chlk.models.common.PermissionsError(topModel, null, permissions));
                return this.PushView(chlk.activities.common.PermissionsErrorPage, result);
            },

            function studyCenterAccessAction(){
                var message = 'Current school doesn\'t support applications, study center, profile explorer';
                return this.generalServerErrorAction(message);
            }
    ])
});
