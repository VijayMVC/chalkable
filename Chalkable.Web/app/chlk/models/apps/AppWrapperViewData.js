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
            String, 'errorTitle',
            String, 'errorMessage',

            [[chlk.models.apps.Application, chlk.models.apps.AppModes]],
            function $(app, mode){

                var fullUrl = app.getCurrentModeUrl() + "&code=" + app.getOauthCode();

                this.setApp(app);
                this.setAppMode(mode);

                var buttons = [];
                switch(mode){
                    case chlk.models.apps.AppModes.EDIT:{
                        buttons.push(chlk.models.common.attachments.ToolbarButton('add-app', '+ Attach'));
                    }break;

                    case chlk.models.apps.AppModes.VIEW:
                    case chlk.models.apps.AppModes.GRADINGVIEW:{
                        buttons.push(chlk.models.common.attachments.ToolbarButton('save-app', 'Save'));
                    }break;

                    case chlk.models.apps.AppModes.MYAPPSVIEW:{
                        var hasMyAppsView = app.getAppAccess().isMyAppsForCurrentRoleEnabled();
                        var myAppsError = !hasMyAppsView || !app.isPersonal();
                        if (myAppsError){
                            this.setMyAppsError(myAppsError);
                            this.setErrorTitle('oops.');
                            var msg = !hasMyAppsView ? 'this app does not have a My Apps view for you.' : 'this app is not installed for you.';
                            this.setErrorMessage(msg);
                        }
                        else{
                            buttons.push(chlk.models.common.attachments.ToolbarButton('new-tab-id', 'New Tab', app.getCurrentModeUrl(), true));
                        }
                    }break;
                }
                BASE(fullUrl, buttons);
            }

        ]);



});
