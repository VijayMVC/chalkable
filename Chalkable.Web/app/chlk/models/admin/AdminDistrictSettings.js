REQUIRE('chlk.models.settings.AdminMessaging');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    /** @class chlk.models.admin.AdminDistrictSettings */

    CLASS('AdminDistrictSettings', [

        [ria.serialize.SerializeProperty('messagingsettings')],
        chlk.models.settings.AdminMessaging, 'messagingSettings',

        [ria.serialize.SerializeProperty('applications')],
        ArrayOf(chlk.models.apps.Application), 'applications'
    ]);
});