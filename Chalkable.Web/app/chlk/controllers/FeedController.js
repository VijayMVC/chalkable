REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.admin.HomePage');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.id.ClassId');



NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FeedController*/
    CLASS(
        'FeedController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.FeedService, 'feedService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.FundsService, 'fundsService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function listAction(postback_, starredOnly_, classId_, pageIndex_) {
            starredOnly_ = starredOnly_ != false;
            var result = this.getFeedItems(postback_, starredOnly_, classId_, pageIndex_)
                .attach(this.validateResponse_());
            return postback_
                    ? this.UpdateView(chlk.activities.feed.FeedListPage, result)
                    : this.PushView(chlk.activities.feed.FeedListPage, result);
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function getFeedItems(postback_, starredOnly_, classId_, pageIndex_){
            return this.announcementService
                .getAnnouncements(pageIndex_ | 0, classId_, starredOnly_)
                .then(function(feedItems){
                    if(!postback_ && starredOnly_ && feedItems.getItems().length == 0)
                        return this.getFeedItems(postback_, false, classId_, pageIndex_);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);

                    var feedModel = new chlk.models.feed.Feed(feedItems);
                    feedModel.setTopData(topModel);
                    feedModel.setStarredOnly(starredOnly_);
                    feedModel.setImportantCount(this.announcementService.getImportantCount());
                    return feedModel;
                }.bind(this))
        },


        //TODO: refactor
        [[Boolean, String]],
        function listAdminAction(update_, gradeLevels_) {
            var res = ria.async.wait([
                    this.feedService.getAdminFeed(gradeLevels_),
                    this.fundsService.getBalance()
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
                    var topModel = new chlk.models.grading.GradeLevelsForTopBar();
                    topModel.setTopItems(gradeLevels);
                    topModel.setSelectedIds(gradeLevels_ ? gradeLevels_.split(',') : []);
                    var model = result[0];
                    model.setTopData(topModel);
                    var markingPeriod = this.getContext().getSession().get('markingPeriod', null);
                    model.setMarkingPeriodName(markingPeriod.getName());
                    model.setBudgetBalance(result[1]);
                    gradeLevels_ && model.setForGradeLevels(true);
                    return model;
                }, this);
            return this[update_ ? 'UpdateView' : 'PushView'](chlk.activities.admin.HomePage, res);
        }


    ])
});
