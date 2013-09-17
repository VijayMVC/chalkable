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
            ArrayOf(chlk.models.apps.AppWrapperToolbarButton), 'buttons',
            chlk.models.apps.AppAttachment, 'app',

            [[chlk.models.apps.Application,
                chlk.models.apps.AppModes,
                ArrayOf(chlk.models.apps.AppWrapperToolbarButton)]],
            function $(app, mode, buttons){
                BASE();

                this.setApp(app);
                this.setAppMode(mode);
                this.setButtons(buttons);
            }


        ]);

    chlk.models.apps.AppWrapperViewData$createAppAttach = function(app){
        var attachBtn = new chlk.models.apps.AppWrapperToolbarButton('add-app', '+ Attach');
        app.setCurrentModeUrl(app.getEditUrl());
        var appWrapperViewData = new chlk.models.apps.AppWrapperViewData(app, chlk.models.apps.AppModes.EDIT, [attachBtn]);
        return new ria.async.DeferredData(appWrapperViewData);
    };

    chlk.models.apps.AppWrapperViewData$createAppView = function(app){
        var saveBtn = new chlk.models.apps.AppWrapperToolbarButton('save-app', 'Save');
        app.setCurrentModeUrl(app.getViewUrl());
        var appWrapperViewData = new chlk.models.apps.AppWrapperViewData(app, chlk.models.apps.AppModes.VIEW, [saveBtn]);
        return new ria.async.DeferredData(appWrapperViewData);
    };
});
