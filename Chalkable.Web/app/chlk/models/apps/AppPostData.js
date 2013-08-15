REQUIRE('chlk.models.id.AppId');
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
            String, 'videoModeUrl',
            String, 'shortDescription',
            String, 'longDescription',

            Boolean, 'hasTeacherMyApps',
            Boolean, 'hasAdminMyApps',
            Boolean, 'hasStudentMyApps',
            Boolean, 'hasParentMyApps',
            Boolean, 'canAttach',
            Boolean, 'showInGradingView',

            Number, 'price',
            Number, 'pricePerClass',
            Number, 'pricePerSchool'
        ]);
});
