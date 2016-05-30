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
            //chlk.models.apps.AppRating, 'appReviews',
            chlk.models.developer.HomeAnalytics, 'appAnalytics',
            chlk.models.common.ChlkDate, 'reportDate',
            ArrayOf(chlk.models.apps.Application), 'devApps',

            [[
                String,
                chlk.models.id.AppId,
                chlk.models.id.AppId,
                chlk.models.apps.AppState,
                String,
                Number,
                chlk.models.common.ChlkDate,
                //chlk.models.apps.AppRating,
                chlk.models.developer.HomeAnalytics,
                ArrayOf(chlk.models.apps.Application)
            ]],
            function $(appName, draftId, liveId_, appState, appThumbnail,
                       appRating,  reportDate,  appAnalytics, devApps){
                BASE();
                this.prepareFields_(appName, draftId, liveId_, appState,
                    appThumbnail, appRating, reportDate, appAnalytics, devApps);
            },

            //[[chlk.models.apps.AppRating]],
            //function $createFromReviews(reviews){
            //    BASE();
            //    this.prepareFields_("", null, null, null, "", null, null,
            //        reviews, new chlk.models.developer.HomeAnalytics(), []);
            //},

            [[
                String,
                chlk.models.id.AppId,
                chlk.models.id.AppId,
                chlk.models.apps.AppState,
                String,
                Number,
                chlk.models.common.ChlkDate,
                //chlk.models.apps.AppRating,
                chlk.models.developer.HomeAnalytics,
                ArrayOf(chlk.models.apps.Application)
            ]],
            function prepareFields_(appName, draftId, liveId_, appState, appThumbnail,
                                    appRating,  reportDate, appAnalytics, devApps){
                this.setAppName(appName);
                this.setDraftAppId(draftId);
                this.setReportDate(reportDate);
                if (liveId_)
                    this.setLiveAppId(liveId_);
                this.setAppThumbnail(appThumbnail);
                this.setAppLive(liveId_ && liveId_.valueOf() != null);
                this.setAppState(appState);
                this.setAppAnalytics(appAnalytics);
                this.setAppRating(appRating);
               // this.setAppReviews(appReviews);
                this.setDevApps(devApps);
            }
        ]);
});
