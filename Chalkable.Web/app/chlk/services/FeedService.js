REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.admin.Home');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FeedService */
    CLASS(
        'FeedService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getAdminFeed() {
                return this.get('Feed/Admin.json', chlk.models.admin.Home, {});
            }
        ])
});