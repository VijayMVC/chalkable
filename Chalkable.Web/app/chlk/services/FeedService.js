REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.admin.Home');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FeedService */
    CLASS(
        'FeedService', EXTENDS(chlk.services.BaseService), [
            [[String]],
            ria.async.Future, function getAdminFeed(gradeLevelIds_) {
                return this.get('Feed/Admin.json', chlk.models.admin.Home, {
                    gradeLevelIds : gradeLevelIds_
                });
            }
        ])
});