REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.admin.Home');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.FeedService */
    CLASS(
        'FeedService', EXTENDS(chlk.services.BaseService), [
            [[Number, String, Boolean]],
            ria.async.Future, function getAdminFeed(pageIndex_, gradeLevelIds_, importantOnly_) {
                return this.get('Feed/DistrictAdminFeed.json', ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), {
                    gradeLevelIds : gradeLevelIds_,
                    start: pageIndex_|0,
                    complete: importantOnly_ ? false : null
                });
            }
        ])
});