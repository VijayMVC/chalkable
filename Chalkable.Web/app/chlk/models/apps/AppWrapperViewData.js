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

            [[chlk.models.apps.Application, chlk.models.apps.AppModes]],
            function $(app, mode){
                BASE();

                var fullUrl = app.getCurrentModeUrl() + "&code=" + app.getOauthCode();
                this.setUrl(fullUrl);
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
                        buttons.push(chlk.models.common.attachments.ToolbarButton('new-tab-id', 'New Tab', app.getCurrentModeUrl(), true));
                    }break;
                }

                this.setToolbarButtons(buttons);
            }

        ]);



});
