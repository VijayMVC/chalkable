REQUIRE('chlk.models.id.SchoolPersonId');
NAMESPACE('chlk.models.settings', function () {
    "use strict";
    /** @class chlk.models.settings.DeveloperSettings*/
    CLASS(
        'DeveloperSettings', [
            chlk.models.id.SchoolPersonId, 'developerId'
        ]);
});
