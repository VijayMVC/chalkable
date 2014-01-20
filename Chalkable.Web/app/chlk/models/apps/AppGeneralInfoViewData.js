REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.apps.AppRating');
REQUIRE('chlk.models.developer.HomeAnalytics');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppGeneralInfoViewData*/
    CLASS(
        'AppGeneralInfoViewData', [
            chlk.models.id.AppId, 'draftAppId',
            chlk.models.id.AppId, 'liveAppId',
            Boolean, 'appLive',
            String, 'appThumbnail',
            String, 'appStatus',
            String, 'appName',
            chlk.models.apps.AppState,  'appState',
            chlk.models.apps.AppRating, 'appRating',
            chlk.models.developer.HomeAnalytics, 'appAnalytics',

            [[
                String,
                chlk.models.id.AppId,
                chlk.models.id.AppId,
                chlk.models.apps.AppState,
                String,
                chlk.models.apps.AppRating,
                chlk.models.developer.HomeAnalytics
            ]],
            function $(appName, draftId, liveId_, appState, appThumbnail, appRating, appAnalytics){
                BASE();
                this.setAppName(appName);
                this.setDraftAppId(draftId);
                if (liveId_)
                    this.setLiveAppId(liveId_);
                this.setAppThumbnail(appThumbnail);
                this.setAppLive(liveId_ && liveId_.valueOf() != null);
                this.setAppState(appState);
                if (!appRating.getRoleRatings())
                    appRating.setRoleRatings([]);
                if (!appRating.getPersonRatings())
                    appRating.setPersonRatings([]);
                this.setAppRating(appRating);
                this.setAppAnalytics(appAnalytics);
            }
        ]);
});
