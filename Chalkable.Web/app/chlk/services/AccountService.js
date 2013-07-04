REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.Account */
    CLASS(
        'AccountService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getRoles() {
                return this.get('/app/data/roles.json', ArrayOf(chlk.models.common.NameId));
            }
        ])
});