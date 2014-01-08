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
            var result = this
                .getFeedItems(postback_, starredOnly_, classId_, pageIndex_)
                .attach(this.validateResponse_());
            return postback_
                    ? this.UpdateView(chlk.activities.feed.FeedListPage, result)
                    : this.PushView(chlk.activities.feed.FeedListPage, result);
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function getFeedItems(postback_, starredOnly_, classId_, pageIndex_){
            return this.announcementService
                .getAnnouncements(pageIndex_ | 0, classId_, starredOnly_)
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    if(!postback_ && starredOnly_ && feedItems.getItems().length == 0)
                        return this.getFeedItems(postback_, false, classId_, pageIndex_);

                    var classes = this.classService.getClassesForTopBar(true);
                    var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(classes, classId_);

                    return new chlk.models.feed.Feed.$create(
                        feedItems,
                        classBarItemsMdl,
                        starredOnly_,
                        this.announcementService.getImportantCount()
                    );
                }, this);
        },

        [[Boolean, String]],
        function listAdminAction(update_, gradeLevels_) {
            var res = ria.async.wait([
                    this.feedService.getAdminFeed(gradeLevels_),
                    this.fundsService.getBalance()
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
                    var gradeLevelsBarMdl = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);
                    var markingPeriod = this.getContext().getSession().get('markingPeriod', null); //todo: move to base controller

                    var model = result[0];
                    model.prepareBaseInfo(gradeLevelsBarMdl, markingPeriod.getName(), result[1], gradeLevels_);
                    return model;
                }, this);
            if (update_)
                return this.UpdateView(chlk.activities.admin.HomePage, res);
            else
                return this.PushView(chlk.activities.admin.HomePage, res);
        }


    ])
});
