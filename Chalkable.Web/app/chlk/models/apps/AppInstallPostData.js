REQUIRE('chlk.models.id.AppId');
NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppInstallPostData*/
    CLASS(
        'AppInstallPostData', [
            chlk.models.id.AppId, 'appId',
            String, 'classes',
            String, 'departments',
            String, 'gradeLevels',
            String, 'roles',
            chlk.models.id.AppInstallGroupId, 'currentPerson'
        ]);
});
