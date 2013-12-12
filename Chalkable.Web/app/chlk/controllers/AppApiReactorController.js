REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.models.common.Button');


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
                        .then(function(result){
                            this.getView().pop();
                            this.getView().pop();
                            return this.Redirect('announcement', 'addAppAttachment', [result]);
                        }, this)
                        .attach(this.validateResponse_());
                }
                else {
                     return this.ShowMsgBox(
                         'App is not ready for closing',
                         'Sorry',[{
                            text: 'Ok',
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                         }],
                         'app-wrapper-error centered'
                     );
                }
            },

            [[Object]],
            function saveMeAction(data){
                if (data.appReady) {
                    this.view.pop();  //close wrapper
                    //if it's student update announcement view for grade
                }
                else {
                    return this.ShowMsgBox(
                        'App is not ready for closing',
                        'Sorry',[{
                            text: 'Ok',
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        }],
                        'app-wrapper-error centered'
                    );
                }
            },


            function closeCurrentAppAction(){
                 this.getView().getCurrent().close();
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
                    params: [],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                }], 'center');
            },

            [[Object]],
            function showPlusAction(data){
                //do partial update and show buttons
                /*
                 IWindow.find('.chalkable-app-action-button').fadeIn();
                * */
            },


            [[Object]],
            function appError(data){

            }
        ]);
});
