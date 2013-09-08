REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketApplication*/
    CLASS(
        'AppMarketApplication', EXTENDS(chlk.models.apps.Application), [

             [ria.serialize.SerializeProperty('developer')],
             chlk.models.developer.DeveloperInfo, 'developerInfo'

        ]);


});
