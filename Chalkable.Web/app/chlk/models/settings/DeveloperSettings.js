REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppId');
NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.DeveloperSettings*/
    CLASS(
        'DeveloperSettings', [
            chlk.models.id.SchoolPersonId, 'developerId',
            chlk.models.id.AppId, 'currentAppId'
        ]);
});
