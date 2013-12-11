REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.apps.AppRating');

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
            chlk.models.apps.AppRating, 'appRating',

            [[String, chlk.models.id.AppId, chlk.models.id.AppId, String, Boolean, String, chlk.models.apps.AppRating]],
            function $(appName, draftId, liveId_, appStatus, isApproved, appThumbnail, appRating){
                BASE();
                this.setAppName(appName);
                this.setDraftAppId(draftId);
                if (liveId_)
                    this.setLiveAppId(liveId_);
                this.setAppThumbnail(appThumbnail);
                this.setAppLive(liveId_ && liveId_.valueOf() != null);
                this.setAppStatus(appStatus);
                this.setApproved(isApproved);
                this.setAppRating(appRating);
            }
        ]);
});
