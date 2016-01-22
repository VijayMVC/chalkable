REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.models.common.Button');
REQUIRE('chlk.activities.apps.AttachAppsDialog');
REQUIRE('chlk.activities.apps.AppShadeDialog');

NAMESPACE('chlk.controllers', function (){

    var waitingForAppResponse = false,
        waitingTimer = null;

    /** @class chlk.controllers.AppApiReactorController */
    CLASS(
        'AppApiReactorController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [[Object]],
            function addAppBeginAction(data) {
                waitingForAppResponse = true;

                waitingTimer && clearTimeout(waitingTimer);

                waitingTimer = setTimeout(function () {
                    if (waitingForAppResponse) {
                        this.appIsNotReadyForClose_();
                    }

                    waitingForAppResponse = false;
                    waitingTimer = null;
                }.bind(this), 10000);

                return null;
            },

            function appResponded_() {
                waitingTimer && clearTimeout(waitingTimer);
                waitingForAppResponse = false;
                waitingTimer = null;
            },

            function appIsNotReadyForClose_() {
                this.ShowMsgBox(
                        'App is not ready for closing',
                        'Sorry',[{
                            text: 'Ok',
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        }],
                        'app-wrapper-error centered'
                    ).then(function () {
                        this.BackgroundUpdateView(chlk.activities.apps.AppWrapperDialog, null, 'unfreeze');
                    }, this);

                return null;
            },

            [[Object]],
            function addMeAction(data){
                this.appResponded_();
                if (data.appReady) {
                    return this.simpleAppAttachAction(data);
                }
                return this.appIsNotReadyForClose_();
            },

            [[Object]],
            function simpleAppAttachAction(data){
                var announcementAppId = new chlk.models.id.AnnouncementApplicationId(data.announcementAppId);
                var announcementId = new chlk.models.id.AnnouncementId(data.announcementId);
                var announcementType = data.announcementType ? new chlk.models.announcement.AnnouncementTypeEnum(data.announcementType) : null;
                return this.appsService
                    .attachApp(announcementAppId, announcementType)
                    .catchError(function(error_){
                        throw new chlk.lib.exception.AppErrorException(error_);
                    }, this)
                    .attach(this.validateResponse_())
                    .then(function(result){
                        this.BackgroundCloseView(chlk.activities.apps.AppWrapperDialog);
                        this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);
                        return this.Redirect('announcement', 'addAppAttachment', [result]);
                    }, this);
            },



            [[Object]],
            function saveMeAction(data){
                this.appResponded_();
                if (data.appReady) {
                    this.BackgroundCloseView(chlk.activities.apps.AppWrapperDialog);
                    //if it's student update announcement view for grade
                    return null;
                }

                return this.appIsNotReadyForClose_();
            },


            [chlk.controllers.SidebarButton('add-new')],
            [[Object]],
            function closeCurrentAppAction(data){
                if (data.attribute_id)
                    this.BackgroundNavigate('announcement', 'refreshAttribute'
                        , [data.announcementId, data.announcementType, data.attribute_id]);
                else
                    if (data.refresh_attached_files)
                        this.BackgroundNavigate('announcement', 'refreshAttachments'
                            , [data.announcementId, data.announcementType]);

                this.getView().getCurrent().close();
                return null;
            },

            [[Object]],
            function closeMeAction(data){
                if (data.force)
                    return this.closeCurrentAppAction(data);

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

                return null;
            },

            [[Object]],
            function appErrorAction(data){
                return this.Redirect('error', 'appError', []);
            },

            [[Object]],
            function showStandardPickerAction(data) {
                this.WidgetStart('apps', 'showStandards', [data.excludeIds || []])
                    .then(function (data) {
                        return data.map(function (_) { return _.serialize(); });
                    }, this)
                    .then(this._replayTo(data));

                return null;
            },

            function _replayTo(data) {
                return function (value) {
                    data.__source.postMessage({action: 'handleResponse', value: value, reqId: data.reqId}, data.__origin);
                }
            },

            [[Object]],
            function showAlertBoxAction(data) {
                this.ShowAlertBox(data.text, data.header || null)
                    .then(this._replayTo(data));

                return null;
            },

            [[Object]],
            function showPromptBoxAction(data) {
                this.ShowPromptBox(data.text
                    , data.header || null
                    , data.inputValue || null
                    , data.inputAttrs || null
                    , data.inputType || null)
                    .then(this._replayTo(data));

                return null;
            },

            [[Object]],
            function showConfirmBoxAction(data) {
                this.ShowConfirmBox(data.text
                    , data.header || null
                    , data.buttonText || null
                    , data.buttonClass || null)
                    .then(this._replayTo(data));

                return null;
            },

            [[Object]],
            function shadeMeAction(data) {
                var res = ria.async.Future.$fromData(null)
                    .then(this._replayTo(data))
                    .then(function () {
                        return chlk.activities.apps.AppShadeDialogModel.$fromData(data.__iframe);
                    });

                return this.ShadeView(chlk.activities.apps.AppShadeDialog, res);
            },

            [[Object]],
            function popMeAction(data) {
                var res = ria.async.Future.$fromData(null)
                    .then(this._replayTo(data))
                    .then(function () {
                        return chlk.activities.apps.AppShadeDialogModel.$fromData(data.__iframe);
                    });

                return this.UpdateView(chlk.activities.apps.AppShadeDialog, res, 'pop-me');
            }
        ]);
});
