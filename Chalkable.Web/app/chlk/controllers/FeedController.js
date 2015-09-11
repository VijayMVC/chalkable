REQUIRE('ria.async.Timer');

REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.feed.FeedListAdminPage');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.controllers', function (){

    var notificationsMinInterval = 60000,
        notificationsInterval = notificationsMinInterval,
        notificationsMaxInterval = 960000,
        notificationsTimer;

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
        chlk.services.NotificationService, 'notificationService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [chlk.controllers.SidebarButton('inbox')],
        function doToListAction(){
            return this.listAction(null, null, true);
        },

        [chlk.controllers.SidebarButton('inbox')],
        function doToListDistrictAdminAction(){
            return this.listDistrictAdminAction(null, null, true);
        },

        OVERRIDE, ria.async.Future, function onAppInit() {
            return BASE()
                .then(function () {
                    //notificationsTimer = ria.async.Timer(notificationsInterval, this.updateNotificationsCounter_);
                    notificationsTimer = setInterval(this.updateNotificationsCounter_, notificationsInterval);

                    var notifications = window[ChlkSessionConstants.NEW_NOTIFICATIONS]|0;
                    this.context.getDefaultView().setNewNotificationCount(notifications);
                }, this);
        },

        [[Object, Number]],
        VOID, function updateNotificationsCounter_(timer_, lag_) {
            this.context
                .getService(chlk.services.NotificationService)
                .getUnShownNotificationCount()
                .then(function(data){
                    this.context.getDefaultView().setNewNotificationCount(data);

                    if(notificationsInterval > notificationsMinInterval){
                        clearInterval(notificationsTimer);
                        notificationsInterval = notificationsMinInterval;
                        notificationsTimer = setInterval(this.updateNotificationsCounter_, notificationsInterval);
                    }

                }, this)
                .catchError(function(){
                    if(notificationsInterval < notificationsMaxInterval){
                        clearInterval(notificationsTimer);
                        notificationsInterval *= 2;
                        notificationsTimer = setInterval(this.updateNotificationsCounter_, notificationsInterval);
                    }
                }, this);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.id.ClassId, Boolean, Boolean, Number]],
        function listAction(classId_, postback_, importantOnly_, start_) {

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
                }], 'center'), null;
            }

            var result = this
                .getFeedItems(postback_, importantOnly_, classId_, start_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListPage, result);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[String, Boolean, Boolean, Number]],
        function listDistrictAdminAction(gradeLevels_, postback_, importantOnly_, start_) {

            //todo : think about go to inow part
            if(!this.hasUserPermission_(chlk.models.people.UserPermissionEnum.CHALKABLE_ADMIN)){
                var text  = 'It looks like you don\'t have the correct permissions defined in iNow \n' +
                    '(Chalkable Admin).\n' +
                    'Without those permissions you cannot use Chalkable Admin POrtal.  \n' +
                    'Please contact your administrator for more information. \n';
                return this.ShowMsgBox(text, 'Error.', [{
                    text: 'LOG OUT',
                    controller: 'account',
                    action: 'logout'
                }], 'center'), null;
            }

            var result = this.getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListAdminPage, result);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.feed.Feed]],
        function getAnnouncementsAction(model) {
            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption(), model.getClassId())
                    .then(function(isMarked){
                        return this.Redirect('feed', 'list', [model.getClassId()]);
                    }, this)

            var result = this
                .getAnnouncements(model.getStart(), model.getClassId(), model.isImportantOnly())
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    return new chlk.models.feed.FeedItems(feedItems);
                });
            return this.UpdateView(chlk.activities.feed.FeedListPage, result, chlk.activities.lib.DontShowLoader());
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.feed.FeedAdmin]],
        function getAnnouncementsDistrictAdminAction(model) {
            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption())
                    .then(function(isMarked){
                        return this.Redirect('feed', 'list', [model.getGradeLevels()]);
                    }, this)

            var result = this
                .getAnnouncements(model.getStart(), model.getClassId(), model.isImportantOnly())
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    return new chlk.models.feed.FeedItems(feedItems);
                });
            return this.UpdateView(chlk.activities.feed.FeedListAdminPage, result, chlk.activities.lib.DontShowLoader());
        },

        Boolean, function canViewFeed(){
            var res = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM)
            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN);
            return res || this.userInRole(chlk.models.common.RoleEnum.STUDENT);
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number]],
        function getFeedItems(postback_, importantOnly_, classId_, start_){
            return this.announcementService
                .getAnnouncements(start_ | 0, classId_, importantOnly_)
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getFeedItems(postback_, false, classId_, start_);

                    var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(null, classId_);

                    return new chlk.models.feed.Feed(
                        feedItems,
                        classBarItemsMdl,
                        importantOnly_,
                        0
                    );
                }, this);
        },

        [[Boolean, Boolean, String, Number]],
        function getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_){
            return this.announcementService
                .getAnnouncementsForAdmin(start_ | 0, gradeLevels_, importantOnly_)
                .attach(this.validateResponse_())
                .then(function(feedItems){
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getAdminFeedItems(postback_, false, gradeLevels_, start_);

                    var glsBarItemsMdl = new chlk.models.grading.GradeLevelsForTopBar(null, gradeLevels_);

                    return new chlk.models.feed.FeedAdmin(
                        feedItems,
                        glsBarItemsMdl,
                        importantOnly_,
                        0
                    );
                }, this);
        }
    ])
});
