REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.activities.feed.FeedListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FeedController*/
    CLASS(
        'FeedController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'feedService',

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.feedService
                .getAnnouncements(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.feed.FeedListPage, result);
        }

    ])
});
