REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.admin.HomePage');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.controllers', function (){

    var notificationsInterval;

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
        chlk.services.NotificationService, 'notificationService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function listAction(postback_, completeOnly_, classId_, pageIndex_) {

            if(!this.canViewFeed()){
                var text  = 'You don\'t have permission (View Lookup, View Classroom or View Classroom (Admin)). ' +
                    'Without those permissions user cann\'t use chalkable. ';
                return this.ShowMsgBox(text, 'Error.', [{
                    text: 'LOG OUT',
                    controller: 'account',
                    action: 'logout'
                }]);
            }

            completeOnly_ = completeOnly_ != false;
            var result = this
                .getFeedItems(postback_, completeOnly_, classId_, pageIndex_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListPage, result);
        },


        Boolean, function canViewFeed(){
            var res = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM)
            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN);
            return res && (!this.userInRole(chlk.models.common.RoleEnum.TEACHER)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_LOOKUP));
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function getFeedItems(postback_, completeOnly_, classId_, pageIndex_){
            if(!notificationsInterval)
                notificationsInterval = setInterval(function(){
                    var result = this.notificationService.getUnShownNotificationCount().then(function(data){
                        this.setNewNotificationCount_(data);
                        return new chlk.models.feed.Feed(null, null, null, data);
                    }, this);
                    this.BackgroundUpdateView(chlk.activities.feed.FeedListPage, result, 'notifications');
                }.bind(this), 5000);
            return this.announcementService
                .getAnnouncements(pageIndex_ | 0, classId_, completeOnly_)
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    if(!postback_ && completeOnly_ && feedItems.length == 0)
                        return this.getFeedItems(postback_, false, classId_, pageIndex_);

                    var classes = this.classService.getClassesForTopBar(true);
                    var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(classes, classId_);

                    return new chlk.models.feed.Feed(
                        feedItems,
                        classBarItemsMdl,
                        completeOnly_,
                        this.getNewNotificationCount_()
                    );
                }, this);
        },

        function stopNotificationsIntervalAction(update_, gradeLevels_) {
            clearInterval(notificationsInterval);
            notificationsInterval = null;
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
