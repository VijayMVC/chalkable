REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SignUpService */
    CLASS(
        'SignUpService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getSignUps(pageIndex_) {
                return this.getPaginatedList('/app/data/signups.json', chlk.models.signup.SignUpInfo, {
                    start: pageIndex_
                });
            }
        ])
});