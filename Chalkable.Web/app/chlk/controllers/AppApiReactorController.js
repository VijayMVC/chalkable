REQUIRE('chlk.controllers.BaseController');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppApiReactorController */
    CLASS(
        'AppApiReactorController', EXTENDS(chlk.controllers.BaseController), [

            [[Object]],
            function addMeAction(data){
                console.info('addMe', data);
                if (data.appReady) {
                    /*that.handlers.attachAction(data.id);
                    that.cantStop = false;
                    !that.stopped_ && that.getWindow().close();*/
                }
                else {
                    /*
                    addMessage({
                        html: String.format('<h2>Error</h2><p>Application is not ready for attaching.</p>'),
                        cls: 'new-style',
                        newStyle: true,
                        zIndex: 100002,
                        buttons: [
                            {
                                text: Msg.OK,
                                cls: 'rounded-state-button green2 x-btn'
                            }
                        ]
                    })*/;
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
