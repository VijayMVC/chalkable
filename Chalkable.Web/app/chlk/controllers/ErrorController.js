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

            function error404Action(error_) {
                var result = new ria.async.DeferredData(chlk.templates.chlkerror.Error404Model.$withMsg(error_ || ''));
                this.view.reset();
                return this.ShadeView(chlk.activities.chlkerror.Error404Page, result);
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
                var topModel = new chlk.models.classes.ClassesForTopBar(this.classService.getClassesForTopBarSync());
                var res = new ria.async.DeferredData(new chlk.models.common.ServerErrorWithClassesModel.$create(topModel, message, controller_, action_, params_));
                this.view.reset();
                return this.ShadeView(chlk.activities.chlkerror.GeneralServerErrorWithClassesPage, res);
            },

            [[String]],
            function generalServerErrorAction(message){
                var res = new ria.async.DeferredData(new chlk.models.common.ServerErrorModel(message));
                this.view.reset();
                return this.ShadeView(chlk.activities.chlkerror.GeneralServerErrorPage, res);
            },

            function appErrorAction(){
                var result = new ria.async.DeferredData(chlk.models.apps.AppWrapperViewData.$createAppErrorViewData(''));
                return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
            },

            [[Array]],
            function permissionsAction(permissions){
                var topModel = new chlk.models.classes.ClassesForTopBar(null);
                var result = new ria.async.DeferredData(new chlk.models.common.PermissionsError(topModel, null, permissions));
                this.view.reset();
                return this.ShadeView(chlk.activities.common.PermissionsErrorPage, result);
            },

            function studyCenterAccessAction(){
                var message = 'Current school doesn\'t support applications, study center, profile explorer';
                return this.generalServerErrorAction(message);
            },


            function LEIntegrationDisabledAction(){
                var message = 'Current school doesn\'t have LE Integration enabled';
                return this.generalServerErrorAction(message);
            },


            [chlk.controllers.NotChangedSidebarButton()],
            [[String]],
            function disabledLinkMsgAction(msg){
                return this.ShowMsgBox(msg, null, null, 'leave-msg'), null;
            },


            function learningEarningsDisabledAction(){
                return this.ShowMsgBox('Chalkable is not able to connect to the Learning Earnings website right now.  Please try again later.', null, null, 'error'), null;
            }
    ])
});
