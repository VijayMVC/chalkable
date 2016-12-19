REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.apps.AppModes');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.common.attachments.BaseAttachmentViewData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppWrapperViewData*/
    CLASS(
        'AppWrapperViewData', EXTENDS(chlk.models.common.attachments.BaseAttachmentViewData), [
            chlk.models.apps.AppModes, 'appMode',
            chlk.models.apps.AppAttachment, 'app',
            Boolean, 'myAppsError',
            Boolean, 'appError',

            Boolean, 'banned',
            String, 'errorTitle',
            String, 'errorMessage',
            Boolean, 'noButtons',

            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [[chlk.models.apps.Application, chlk.models.apps.AppModes, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
            function $(app, mode, announcementType_, noButtons_){

                var fullUrl = app.getCurrentModeUrl() + "&token=" + encodeURIComponent(app.getToken());

                this.setApp(app);
                this.setAppMode(mode);
                noButtons_ && this.setNoButtons(noButtons_);

                if(announcementType_)
                    this.setAnnouncementType(announcementType_);

                var buttons = [];
                switch(mode){
                    case chlk.models.apps.AppModes.EDIT:{
                        buttons.push(chlk.models.common.attachments.ToolbarButton('add-app', 'Attach'));
                    }break;

                    case chlk.models.apps.AppModes.VIEW:
                    case chlk.models.apps.AppModes.GRADINGVIEW:{
                        buttons.push(chlk.models.common.attachments.ToolbarButton('save-app', 'Save'));
                    }break;

                    case chlk.models.apps.AppModes.MYAPPSVIEW:{
                        var hasMyAppsView = app.getAppAccess().isMyAppsForCurrentRoleEnabled();
                        var myAppsError = !hasMyAppsView;
                        if (myAppsError){
                            this.setMyAppsError(myAppsError);
                            this.setErrorTitle('oops.');
                            var msg = 'this app does not have a My Apps view for you.';
                            this.setErrorMessage(msg);
                        }
                    }break;
                }
                BASE(fullUrl, buttons);
            },

            [[String]],
            function $createAppBannedViewData(url){
                BASE(url, []);
                this.setBanned(true);
            },

            [[String]],
            function $createAppErrorViewData(url){
                BASE(url, []);
                this.setAppError(true);
            }
        ]);
});
