REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.Application');
NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.ApplicationService */
    CLASS(
        'AppMarketService', EXTENDS(chlk.services.BaseService), [

            function $() {
                BASE();
            },

            [[Number]],
            ria.async.Future, function getApps(pageIndex_) {
                return this.getPaginatedList('Application/List.json', chlk.models.apps.Application, {
                        start: pageIndex_
                    })
            }
        ])
});