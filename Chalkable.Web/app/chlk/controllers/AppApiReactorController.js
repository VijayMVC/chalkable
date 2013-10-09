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
                            this.view.pop();  //close wrapper
                            this.view.pop();  //close attach app dialog
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
            [[Object]],
            function showPlusAction(data){
                //do partial update and show buttons
                console.info('addMe', data);
                /*
                 IWindow.find('.chalkable-app-action-button').fadeIn();
                * */
            }
        ]);
});
