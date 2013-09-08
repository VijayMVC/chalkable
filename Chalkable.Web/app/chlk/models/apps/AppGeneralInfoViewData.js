REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppGeneralInfoViewData*/
    CLASS(
        'AppGeneralInfoViewData', [
            chlk.models.id.AppId, 'draftAppId',
            chlk.models.id.AppId, 'liveAppId',
            Boolean, 'appLive',
            Boolean, 'approved',
            String, 'appThumbnail',
            String, 'appStatus',
            String, 'appName',

            [[String, chlk.models.id.AppId, chlk.models.id.AppId, String, Boolean, String]],
            function $(appName, draftId, liveId_, appStatus, isApproved, appThumbnail){
                BASE();
                this.setAppName(appName);
                this.setDraftAppId(draftId);
                if (liveId_)
                    this.setLiveAppId(liveId_);
                this.setAppThumbnail(appThumbnail);
                this.setAppLive(liveId_ && liveId_.valueOf() != null);
                this.setAppStatus(appStatus);
                this.setApproved(isApproved);
            }
        ]);
});
