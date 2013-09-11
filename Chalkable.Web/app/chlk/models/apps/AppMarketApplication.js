REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppInstallGroup');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketApplication*/
    CLASS(
        'AppMarketApplication', EXTENDS(chlk.models.apps.Application), [

             [ria.serialize.SerializeProperty('developer')],
             chlk.models.developer.DeveloperInfo, 'developerInfo',

            [ria.serialize.SerializeProperty('installedforpersonsgroup')],
            ArrayOf(chlk.models.apps.AppInstallGroup), 'installedForGroups',

            [ria.serialize.SerializeProperty('isinstalledonlyforme')],
            Boolean, 'installedOnlyForCurrentUser'


        ]);


});
