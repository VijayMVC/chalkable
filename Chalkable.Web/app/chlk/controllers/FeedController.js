REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.admin.HomePage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FeedController*/
    CLASS(
        'FeedController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.FeedService, 'feedService',

        [ria.mvc.Inject],
        chlk.services.FundsService, 'fundsService',

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
        },

        [[String]],
        function adminAction(gradeLevels_) {
            var res = ria.async.wait([
                    this.feedService.getAdminFeed(),
                    this.fundsService.getBalance()
                ]).then(function(result){
                    var model = result[0];
                    var markingPeriod = this.getContext().getSession().get('markingPeriod', null);
                    model.setMarkingPeriodName(markingPeriod.getName());
                    model.setBudgetBalance(result[1]);
                    gradeLevels_ && model.setForGradeLevels(true);
                    return model;
                }, this);
            return this.PushView(chlk.activities.admin.HomePage, res);
        }


    ])
});
