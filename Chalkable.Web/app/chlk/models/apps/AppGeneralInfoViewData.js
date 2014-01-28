REQUIRE('chlk.models.id.AppId');
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
            Number, 'appRating',
            chlk.models.apps.AppState,  'appState',
            chlk.models.common.PaginatedList, 'appReviews',
            chlk.models.developer.HomeAnalytics, 'appAnalytics',

            [[
                String,
                chlk.models.id.AppId,
                chlk.models.id.AppId,
                chlk.models.apps.AppState,
                String,
                Number,
                chlk.models.common.PaginatedList,
                chlk.models.developer.HomeAnalytics
            ]],
            function $(appName, draftId, liveId_, appState, appThumbnail, appRating, appReviews, appAnalytics){
                BASE();
                this.setAppName(appName);
                this.setDraftAppId(draftId);
                if (liveId_)
                    this.setLiveAppId(liveId_);
                this.setAppThumbnail(appThumbnail);
                this.setAppLive(liveId_ && liveId_.valueOf() != null);
                this.setAppState(appState);
                this.setAppAnalytics(appAnalytics);
                this.setAppRating(appRating);
                this.setAppReviews(appReviews);
            }
        ]);
});
