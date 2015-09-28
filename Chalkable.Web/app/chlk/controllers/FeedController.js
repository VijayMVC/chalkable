REQUIRE('ria.async.Timer');

REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.feed.FeedListAdminPage');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradingPeriodService');
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

        [ria.mvc.Inject],
        chlk.services.GradingPeriodService, 'gradingPeriodService',

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
        [[chlk.models.id.ClassId, Boolean, Boolean, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            chlk.models.id.GradingPeriodId, Object, Boolean]],
        function listAction(classId_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_) {

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

            annType_ = annType_ instanceof chlk.models.announcement.AnnouncementTypeEnum ? annType_ : null;

            var result = this
                .getFeedItems(postback_, importantOnly_, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListPage, result);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[String, Boolean, Boolean, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.GradingPeriodId,
            Object, Boolean]],
        function listDistrictAdminAction(gradeLevels_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_) {

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

            annType_ = annType_ instanceof chlk.models.announcement.AnnouncementTypeEnum ? annType_ : null;

            var result = this.getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListAdminPage, result);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.feed.Feed]],
        function getAnnouncementsAction(model) {
            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption(), model.getClassId())
                    .then(function(isMarked){
                        return this.Redirect('feed', 'list', [model.getClassId(), null, true, 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest()]);
                    }, this);

            if(model.getSubmitType() == 'sort')
                return this.Redirect('feed', 'list', [model.getClassId(), null, model.isImportantOnly(), 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest()]);

            var result = this.announcementService
                .getAnnouncements(model.getStart(), model.getClassId(), model.isImportantOnly(), model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest())
                .attach(this.validateResponse_())
                .then(function(model){
                    return new chlk.models.feed.FeedItems(model.getItems());
                });
            return this.UpdateView(chlk.activities.feed.FeedListPage, result, chlk.activities.lib.DontShowLoader());
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.feed.FeedAdmin]],
        function getAnnouncementsDistrictAdminAction(model) {
            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption())
                    .then(function(isMarked){
                        return this.Redirect('feed', 'list', [model.getGradeLevels(), null, true, 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest()]);
                    }, this);

            if(model.getSubmitType() == 'sort')
                return this.Redirect('feed', 'list', [model.getGradeLevels(), null, true, 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest()]);

            var result = this.announcementService
                .getAnnouncementsForAdmin(model.getStart(), model.getGradeLevels(), model.isImportantOnly(), model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.isLatest())
                .attach(this.validateResponse_())
                .then(function(model){
                    return new chlk.models.feed.FeedItems(model.getItems());
                });
            return this.UpdateView(chlk.activities.feed.FeedListAdminPage, result, chlk.activities.lib.DontShowLoader());
        },

        Boolean, function canViewFeed(){
            var res = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM)
            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN);
            return res || this.userInRole(chlk.models.common.RoleEnum.STUDENT);
        },

        [[Boolean, Boolean, chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            chlk.models.id.GradingPeriodId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function getFeedItems(postback_, importantOnly_, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_){
            return ria.async.wait([
                this.announcementService.getAnnouncements(start_ | 0, classId_, importantOnly_, startDate_, endDate_, gradingPeriodId_, annType_, latest_),
                this.gradingPeriodService.getList()
            ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0], gradingPeriods = result[1];
                    var feedItems = model.getItems();
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getFeedItems(postback_, false, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_);

                    model.setGradingPeriods(gradingPeriods);
                    var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(null, classId_);
                    var gp = this.getCurrentGradingPeriod();
                    if(model.getStartDate() && (model.getStartDate().getDate() < gp.getStartDate().getDate() || model.getStartDate().getDate() > gp.getEndDate().getDate()))
                        model.setStartDate(gp.getStartDate());
                    if(model.getEndDate() && (model.getEndDate().getDate() > gp.getEndDate().getDate() || model.getEndDate().getDate() < gp.getStartDate().getDate()))
                        model.setEndDate(gp.getEndDate());
                    model.setTopData(classBarItemsMdl);
                    importantOnly_ !== undefined && model.setImportantOnly(importantOnly_);

                    return model;
                }, this);
        },

        [[Boolean, Boolean, String, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.GradingPeriodId,
            chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_){
            return ria.async.wait([
                this.announcementService.getAnnouncementsForAdmin(start_ | 0, gradeLevels_, importantOnly_, startDate_, endDate_, gradingPeriodId_, annType_, latest_),
                this.gradingPeriodService.getList()
            ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0], gradingPeriods = result[1];
                    var feedItems = model.getItems();
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getAdminFeedItems(postback_, false, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, latest_);

                    model.setGradingPeriods(gradingPeriods);
                    var glsBarItemsMdl = new chlk.models.grading.GradeLevelsForTopBar(null, gradeLevels_);
                    model.setTopData(glsBarItemsMdl);
                    importantOnly_ !== undefined && model.setImportantOnly(importantOnly_);

                    return model;
                }, this);
        }
    ])
});
