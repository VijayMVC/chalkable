REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ApplicationService */
    CLASS(
        'ApplicationService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getApps(pageIndex_) {
                return this.getPaginatedList('/app/data/apps.json', chlk.models.apps.Application, pageIndex_);
            }
        ])
});