REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.apps.AppModes');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppWrapperToolbarButton*/
    CLASS(
        'AppWrapperToolbarButton', [
            String, 'title',
            String, 'id',
            String, 'url',
            Boolean, 'targetBlank',

            [[String, String, String, Boolean]],
            function $(id, title, url_, targetBlank_){
                BASE();
                this.setId(id);
                this.setTitle(title);
                if (url_)
                    this.setUrl(url_);
                if (targetBlank_)
                    this.setTargetBlank(targetBlank_);
            }
        ]);

    /** @class chlk.models.apps.AppWrapperViewData*/
    CLASS(
        'AppWrapperViewData', [
            chlk.models.apps.AppModes, 'appMode',
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'toolbarButtons',
            chlk.models.apps.AppAttachment, 'app',

            [[chlk.models.apps.Application, chlk.models.apps.AppModes]],
            function $(app, mode){
                BASE();



                var fullUrl = app.getCurrentModeUrl() + "&code=" + app.getOauthCode();
                app.setCurrentModeUrl(fullUrl);
                this.setApp(app);
                this.setAppMode(mode);

                var buttons = [];
                switch(mode){
                    case chlk.models.apps.AppModes.EDIT:{
                        buttons.push(new chlk.models.apps.AppWrapperToolbarButton('add-app', '+ Attach'));
                    }break;

                    case chlk.models.apps.AppModes.VIEW:
                    case chlk.models.apps.AppModes.GRADINGVIEW:{
                        buttons.push(new chlk.models.apps.AppWrapperToolbarButton('save-app', 'Save'));
                    }break;

                    case chlk.models.apps.AppModes.MYAPPSVIEW:{
                        buttons.push(new chlk.models.apps.AppWrapperToolbarButton('new-tab-id', 'New Tab', app.getCurrentModeUrl(), true));
                    }break;
                }

                this.setToolbarButtons(buttons);

            }

        ]);



});
