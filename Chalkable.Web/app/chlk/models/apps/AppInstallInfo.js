REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.ApplicationInstallId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppInstallInfo*/
    CLASS(
        'AppInstallInfo', [

            [ria.serialize.SerializeProperty('applicationinstallid')],
            chlk.models.id.ApplicationInstallId, 'appInstallId',

            [ria.serialize.SerializeProperty('installationownerid')],
            chlk.models.id.SchoolPersonId, 'installationOwnerId',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'owner',

            [ria.serialize.SerializeProperty('personid')],
            chlk.models.id.SchoolPersonId, 'personId'
    ])

});
