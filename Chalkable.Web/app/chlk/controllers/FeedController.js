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

        [chlk.controllers.SidebarButton('inbox')],
        function doToListAction(){
            return this.listAction(null, true);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function listAction(postback_, importantOnly_, classId_, pageIndex_) {

            //todo : think about go to inow part
            if(!this.canViewFeed()){
                var text  = 'It looks like you don\'t have the correct permissions defined in iNow \n' +
                    '(View Lookup, View Classroom or View Classroom (Admin)).\n' +
                    'Without those permissions you cannot use Chalkable.  \n' +
                    'Please contact your administrator for more information. \n';
                return this.ShowMsgBox(text, 'Error.', [{
                    text: 'LOG OUT',
                    controller: 'account',
                    action: 'logout'
                }], 'center');
            }

            var result = this
                .getFeedItems(postback_, importantOnly_, classId_, pageIndex_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListPage, result);
        },


        Boolean, function canViewFeed(){
            var res = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM)
            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN);
            return res || this.userInRole(chlk.models.common.RoleEnum.STUDENT);
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function getFeedItems(postback_, importantOnly_, classId_, pageIndex_){
            if(!window.notificationsInterval)
                window.notificationsInterval = setInterval(function(){
                    var result = this.notificationService.getUnShownNotificationCount().then(function(data){
                        this.setNewNotificationCount_(data);
                        return new chlk.models.feed.Feed(null, null, null, data);
                    }, this);
                    this.BackgroundUpdateView(chlk.activities.feed.FeedListPage, result, 'notifications');
                }.bind(this), 60000);
            return this.announcementService
                .getAnnouncements(pageIndex_ | 0, classId_, importantOnly_)
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getFeedItems(postback_, false, classId_, pageIndex_);

                    var classes = this.classService.getClassesForTopBar(true);
                    var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                    var firstLogin = this.getContext().getSession().get(ChlkSessionConstants.FIRST_LOGIN, false);
                    this.getContext().getSession().set(ChlkSessionConstants.FIRST_LOGIN, false);

                    return new chlk.models.feed.Feed(
                        feedItems,
                        classBarItemsMdl,
                        importantOnly_,
                        this.getNewNotificationCount_(),
                        firstLogin
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
                    var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD, null); //todo: move to base controller

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
