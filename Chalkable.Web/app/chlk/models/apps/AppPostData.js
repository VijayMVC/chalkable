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
            String, 'name',
            String, 'url',
            String, 'videoDemoUrl',
            String, 'shortDescription',
            String, 'longDescription',

            Boolean, 'hasTeacherMyApps',
            Boolean, 'hasAdminMyApps',
            Boolean, 'hasStudentMyApps',
            Boolean, 'hasParentMyApps',
            Boolean, 'canAttach',
            Boolean, 'showInGradingView',

            Number, 'costPerUser',
            Number, 'costPerSchool',
            Number, 'costPerClass',

            Boolean, 'free',
            Boolean, 'classFlatRateEnabled',
            Boolean, 'schoolFlatRateEnabled',

            chlk.models.id.PictureId, 'appIconId',
            chlk.models.id.PictureId, 'appBannerId',
            String, 'appScreenShotIds'
        ]);
});
