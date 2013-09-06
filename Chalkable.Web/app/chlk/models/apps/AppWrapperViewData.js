REQUIRE('chlk.models.apps.Application');
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
            chlk.models.apps.Application, 'app',
            chlk.models.id.AnnouncementId, 'announcementId',


            [[chlk.models.apps.Application,
                chlk.models.apps.AppModes,
                ArrayOf(chlk.models.apps.AppWrapperToolbarButton),
                chlk.models.id.AnnouncementId]],
            function $(app, mode, buttons, announcementId){
                this.setApp(app);
                this.setAppMode(mode);
                this.setButtons(buttons);
                this.setAnnouncementId(announcementId);

            }


        ]);

    chlk.models.apps.AppWrapperViewData$createAppAttach = function(announcementId, app){
        var attachBtn = new chlk.models.apps.AppWrapperToolbarButton('add-app', '+ Attach');
        var appWrapperViewData = new chlk.models.apps.AppWrapperViewData(app,
            chlk.models.apps.AppModes.EDIT, [attachBtn], announcementId);
        return new ria.async.DeferredData(appWrapperViewData);
    };

});
