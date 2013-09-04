REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.AppMarketService */
    CLASS(
        'AppMarketService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInstalledApps(personId) {
                //return this.getPaginatedList('Application/List.json', chlk.models.apps.AppMarketApplication, {
                return this.getPaginatedList('AppMarket/ListInstalled.json', chlk.models.apps.AppMarketApplication, {
                        personId: personId.valueOf()
                });
            }
        ])
});