REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.chlkerror.Error404Page');
REQUIRE('chlk.models.apps.AppErrorViewData');
REQUIRE('chlk.activities.chlkerror.AppErrorDialog');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ErrorController*/
    CLASS(
        'ErrorController', EXTENDS(chlk.controllers.BaseController), [

            function generalErrorAction() {
                var result = new ria.async.DeferredData(new chlk.models.Success());
                return this.PushView(chlk.activities.chlkerror.Error404Page, result);
            },

            function appErrorAction(){
                var result = new ria.async.DeferredData(new chlk.models.apps.AppErrorViewData('someemail'));
                return this.ShadeView(chlk.activities.chlkerror.AppErrorDialog, result);
            }
    ])
});
