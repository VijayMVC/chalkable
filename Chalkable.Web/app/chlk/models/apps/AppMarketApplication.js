REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.apps.AppInstallInfo');
REQUIRE('chlk.models.apps.AppRating');
REQUIRE('chlk.models.apps.BannedAppData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketApplication*/
    CLASS(
        'AppMarketApplication', EXTENDS(chlk.models.apps.Application), [

            [ria.serialize.SerializeProperty('installedforpersonsgroup')],
            ArrayOf(chlk.models.apps.AppInstallGroup), 'installedForGroups',

            [ria.serialize.SerializeProperty('isinstalledonlyforme')],
            Boolean, 'installedOnlyForCurrentUser',

            [ria.serialize.SerializeProperty('applicationinstalls')],
            ArrayOf(chlk.models.apps.AppInstallInfo), 'applicationInstalls',

            Boolean, 'uninstallable',
            Boolean, 'selfInstalled',
            Boolean, 'personal',
            String,  'applicationInstallIds',

            [ria.serialize.SerializeProperty('applicationrating')],
            chlk.models.apps.AppRating, 'applicationRating',

            chlk.models.apps.BannedAppData, 'banInfo'

        ]);


});
