REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.activities.feed.FeedListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FeedController*/
    CLASS(
        'FeedController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.announcementService
                .getAnnouncements(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.feed.FeedListPage, result);
        },

        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.announcementService
                .getAnnouncements(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.feed.FeedListPage, result);
        }


    ])
});
