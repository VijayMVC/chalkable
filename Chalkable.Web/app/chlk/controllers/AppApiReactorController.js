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
                console.info('addMe', data);
                if (data.appReady) {
                    var announcementId = new chlk.models.id.AnnouncementId(data.announcementId);
                    var appId = new chlk.models.id.AppId(data.appId);

                    //close wrapper, close attach dialog, update announcement attachments
                    var result = this.appsService.addAppToAnnouncement(appId ,announcementId);
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
            function closeMeAction(data){
                console.info('addMe', data);
                /*
                 that.handlers.closeAction(null, {id: data.applicationid});
                 that.cantStop = false;
                 !that.stopped_ && that.getWindow().close();
                 */
            },

            [[Object]],
            function saveMeAction(data){
                console.info('saveMe', data);
                /*
                 if (e.data.appReady) {
                 that.cantStop = false;
                 !that.stopped_ && that.getWindow().close();
                 if (USER_ROLE === UserRoles.STUDENT && that.appMode === APP_MODES.VIEW) {
                 window.location.reload();
                 }
                 } else {
                 addMessage({
                 html: '<h2>Error</h2>' +
                 '<p>Application is not ready for saving.</p>',
                 cls: 'new-style',
                 newStyle: true,
                 zIndex: 100002,
                 buttons: [
                 {
                 text: 'OK',
                 cls: 'rounded-state-button green2 x-btn'
                 }
                 ]
                 });
                 }
                 */
            },
            [[Object]],
            function showPlusAction(data){
                console.info('addMe', data);
                /*
                 IWindow.find('.chalkable-app-action-button').fadeIn();
                * */
            }
        ]);
});
