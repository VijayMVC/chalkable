REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.models.common.Button');
REQUIRE('chlk.activities.apps.AttachAppDialog');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppApiReactorController */
    CLASS(
        'AppApiReactorController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [[Object]],
            function addMeAction(data){
                if (data.appReady) {
                    var announcementAppId = new chlk.models.id.AnnouncementApplicationId(data.announcementAppId);
                    var announcementId = new chlk.models.id.AnnouncementId(data.announcementId);
                    return this.appsService
                        .attachApp(announcementAppId)
                        .catchError(function(error_){
                            throw new chlk.lib.exception.AppErrorException(error_);
                        }, this)
                        .attach(this.validateResponse_())
                        .then(function(result){
                            this.BackgroundCloseView(chlk.activities.apps.AppWrapperDialog);
                            this.BackgroundCloseView(chlk.activities.apps.AttachAppDialog);
                            return this.Redirect('announcement', 'addAppAttachment', [result]);
                        }, this);
                }
                else {
                     return this.ShowMsgBox(
                         'App is not ready for closing',
                         'Sorry',[{
                            text: 'Ok',
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                         }],
                         'app-wrapper-error centered'
                     ), null;
                }
            },

            [[Object]],
            function saveMeAction(data){
                if (data.appReady) {
                    this.view.pop();  //close wrapper
                    //if it's student update announcement view for grade
                    return null;
                }
                else {
                    return this.ShowMsgBox(
                        'App is not ready for closing',
                        'Sorry',[{
                            text: 'Ok',
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        }],
                        'app-wrapper-error centered'
                    ), null;
                }
            },


            [[Object]],
            function closeCurrentAppAction(data){
                this.getView().getCurrent().close();
                return null;
            },

            [[Object]],
            function closeMeAction(data){
                 return this.ShowMsgBox('Close without attaching the app?', 'just checking.', [{
                    text: 'CANCEL',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf()
                }, {
                    text: 'DON\'T ATTACH',
                    controller: 'appapireactor',
                    action: 'closeCurrentApp',
                    params: [data],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                }], 'center'), null;
            },

            [[Object]],
            function showPlusAction(data){
                //do partial update and show buttons
                /*
                 IWindow.find('.chalkable-app-action-button').fadeIn();
                * */

                return null;             },

            [[Object]],
            function appErrorAction(data){
                return this.Redirect('error', 'appError', []);
            }
        ]);
});
