REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.PictureId');
NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppPostData*/
    CLASS(
        'AppPostData', [
            chlk.models.id.AppId, 'id',
            Boolean, 'draft',
            String, 'gradeLevels',
            String, 'permissions',
            String, 'categories',
            String, 'platforms',
            String, 'name',
            String, 'url',
            String, 'videoDemoUrl',
            String, 'shortDescription',
            String, 'longDescription',
            String, 'standards',

            Boolean, 'teacherMyAppsEnabled',
            Boolean, 'adminMyAppsEnabled',
            Boolean, 'studentMyAppsEnabled',
            Boolean, 'parentMyAppsEnabled',
            Boolean, 'attachEnabled',
            Boolean, 'showInGradingViewEnabled',

            Number, 'costPerUser',
            Number, 'costPerSchool',
            Number, 'costPerClass',

            Boolean, 'free',
            Boolean, 'classFlatRateEnabled',
            Boolean, 'schoolFlatRateEnabled',

            chlk.models.id.PictureId, 'appIconId',
            chlk.models.id.PictureId, 'appBannerId',
            String, 'appScreenshots',


            //for check
            String, 'developerWebSite',
            String, 'developerName'
        ]);
});
