REQUIRE('ria.async.Timer');

REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.FeedService');
REQUIRE('chlk.services.NotificationService');
REQUIRE('chlk.activities.feed.FeedListPage');
REQUIRE('chlk.activities.feed.FeedListAdminPage');
REQUIRE('chlk.activities.feed.FeedPrintingDialog');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.SchoolYearService');
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
        chlk.services.SchoolYearService, 'schoolYearService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.ReportingService, 'reportingService',

        [ria.mvc.Inject],
        chlk.services.NotificationService, 'notificationService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.GradingPeriodService, 'gradingPeriodService',

        [chlk.controllers.SidebarButton('inbox')],
        function doToListAction(){
            var classId = this.getCurrentClassId();
            return this.Redirect('feed', 'list', [classId, null, true]);
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

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.ClassId]],
        function viewImportedAction(classId){
            var announcements = this.getContext().getSession().get(ChlkSessionConstants.CREATED_ANNOUNCEMENTS, []);
            this.getContext().getSession().remove(ChlkSessionConstants.CREATED_ANNOUNCEMENTS);
            return this.listAction(classId, null, null, null, null, null, null, null, null, null, null, announcements);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.ClassId, Boolean, Boolean, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            chlk.models.id.GradingPeriodId, Object, chlk.models.announcement.FeedSortTypeEnum, Boolean]],
        function listForProfileAction(classId_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_){
            return this.listAction(classId_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, true);
        },

        function parseEnumValue_(enumType, valToParse_){
            if(!(valToParse_ instanceof enumType )){
                var val = parseInt(valToParse_, 10);
                if(val || val === 0)
                    valToParse_ = new enumType(valToParse_);
                else
                    valToParse_ = null;
            }
            return valToParse_;
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.id.ClassId, Boolean, Boolean, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            chlk.models.id.GradingPeriodId, Object, Object, Boolean, Boolean, Object]],
        function listAction(classId_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, isProfile_, createdAnnouncements_) {

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

            sortType_ = this.parseEnumValue_(chlk.models.announcement.FeedSortTypeEnum, sortType_);
            annType_ = this.parseEnumValue_(chlk.models.announcement.AnnouncementTypeEnum, annType_);

            var result = this
                .getFeedItems(postback_, importantOnly_, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, isProfile_, createdAnnouncements_)
                .attach(this.validateResponse_());

            if(isProfile_)
                return this.UpdateView(this.getView().getCurrent().getClass(), result);

            return this.PushOrUpdateView(chlk.activities.feed.FeedListPage, result);
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[String, Boolean, Boolean, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.GradingPeriodId,
            Object, Object, Boolean]],
        function listDistrictAdminAction(gradeLevels_, postback_, importantOnly_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_) {

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

            sortType_ = this.parseEnumValue_(chlk.models.announcement.FeedSortTypeEnum, sortType_);
            annType_ = this.parseEnumValue_(chlk.models.announcement.AnnouncementTypeEnum, annType_);

            var result = this.getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.feed.FeedListAdminPage, result);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.feed.Feed]],
        function getAnnouncementsForProfileAction(model){
            return this.getAnnouncementsAction(model);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.feed.Feed]],
        function getAnnouncementsAction(model) {
            var res;
            if(model.getSubmitType() == 'copy'){
                res = this.announcementService.copy(model.getClassId(), model.getToClassId(), model.getSelectedAnnouncements(), model.getCopyStartDate())
                    .then(function(model){
                        this.userTrackingService.copiedActivities();
                        return model;
                    }, this);
                return this.UpdateView(this.getView().getCurrent().getClass(), res, 'announcements-copy');
            }
            if(model.getSubmitType() == 'adjust'){
                res = this.announcementService.adjustDates(model.getClassId(), model.getSelectedAnnouncements(), model.getAdjustStartDate())
                    .then(function(data){
                        return this.getFeedItems(false, model.isImportantOnly(), model.getClassId(), 0, model.getStartDate(), model.getEndDate(),
                            model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), model.isToSet(), false, JSON.parse(model.getSelectedAnnouncements()));
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(this.getView().getCurrent().getClass(), res);
            }

            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption(), model.getClassId(), model.getAnnType())
                    .then(function(isMarked){
                        return this.Redirect('feed', model.isInProfile() ? 'listForProfile' : 'list', [model.getClassId(), null, true, 0, model.getStartDate(), model.getEndDate(),
                            model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), null]);
                    }, this);

            if(model.getSubmitType() == 'sort')
                return this.Redirect('feed', model.isInProfile() ? 'listForProfile' : 'list', [model.getClassId(), null, model.isImportantOnly(), 0, model.getStartDate(), model.getEndDate(),
                    model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), model.isToSet()]);


            var result = this.getAnnouncements_(model)
                .attach(this.validateResponse_())
                .then(function(model){
                    return new chlk.models.feed.FeedItems(model.getItems());
                });
            return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.feed.Feed]],
        function getAnnouncements_(model){
            return model.isInProfile() && model.getClassId()
                ? this.announcementService.getAnnouncementsForClassProfile(model.getClassId(), model.getStart() | 0,
                    model.isImportantOnly(), model.getStartDate(),  model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.getSortType())
                : this.announcementService.getAnnouncements(model.getStart(), model.getClassId(), model.isImportantOnly(), model.getStartDate(), model.getEndDate(),
                    model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), model.isToSet());
        },


        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.feed.FeedAdmin]],
        function getAnnouncementsDistrictAdminAction(model) {
            if(model.getSubmitType() == 'markDone')
                return this.announcementService.markDone(model.getMarkDoneOption(), null, model.getAnnType())
                    .then(function(isMarked){
                        return this.Redirect('feed', 'list', [model.getGradeLevels(), null, true, 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.getSortType()]);
                    }, this);

            if(model.getSubmitType() == 'sort')
                return this.Redirect('feed', 'list', [model.getGradeLevels(), null, model.isImportantOnly(), 0, model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), model.isToSet()]);

            var result = this.announcementService
                .getAnnouncementsForAdmin(model.getStart(), model.getGradeLevels(), model.isImportantOnly(), model.getStartDate(), model.getEndDate(), model.getGradingPeriodId(), model.getAnnType(), model.getSortType(), model.isToSet())
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
            chlk.models.id.GradingPeriodId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.announcement.FeedSortTypeEnum, Boolean, Boolean, Object]],
        function getFeedItems(postback_, importantOnly_, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, isProfile_, createdAnnouncements_){

            var annsList = isProfile_ && classId_
                ? this.announcementService.getAnnouncementsForClassProfile(classId_, start_ | 0, importantOnly_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_)
                : this.announcementService.getAnnouncements(start_ | 0, classId_, importantOnly_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, createdAnnouncements_);

            var gps = isProfile_ && classId_
                ? this.gradingPeriodService.getListByClassId(classId_)
                : this.gradingPeriodService.getList();

            var res;

            if(classId_ && classId_.valueOf() && this.userIsTeacher())
                res = ria.async.wait([
                    annsList,
                    gps,
                    this.schoolYearService.listOfSchoolYearClasses(),
                    this.classService.getScheduledDays(classId_)
                ]);
            else
                res = ria.async.wait([
                    annsList,
                    gps,
                    this.schoolYearService.listOfSchoolYearClasses()
                ]);


            return res
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0], gradingPeriods = result[1], classesByYears = result[2], classScheduledDays = result[3];

                    if(isProfile_){
                        model.setInProfile(true);
                        model.setClassId(classId_);
                    }else{
                        var feedItems = model.getItems();
                        if(!postback_ && importantOnly_ && feedItems.length == 0)
                            return this.getFeedItems(postback_, false, classId_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_, isProfile_);

                        var classBarItemsMdl = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        model.setTopData(classBarItemsMdl);
                    }

                    if(classId_ && classId_.valueOf()){
                        var staringEnabled = !this.userIsAdmin() && this.isAssignedToClass_(classId_);
                        model.setStaringDisabled(!staringEnabled);
                    }

                    model.setGradingPeriods(gradingPeriods);
                    model.setClassesByYears(classesByYears);
                    classScheduledDays && model.setClassScheduledDays(classScheduledDays);
                    importantOnly_ !== undefined && model.setImportantOnly(importantOnly_);

                    return model;
                }, this);
        },

        [[Boolean, Boolean, String, Number, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.GradingPeriodId,
            chlk.models.announcement.AnnouncementTypeEnum, chlk.models.announcement.FeedSortTypeEnum, Boolean]],
        function getAdminFeedItems(postback_, importantOnly_, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_){
            return ria.async.wait([
                this.announcementService.getAnnouncementsForAdmin(start_ | 0, gradeLevels_, importantOnly_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_),
                this.gradingPeriodService.getList()
            ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0], gradingPeriods = result[1];
                    var feedItems = model.getItems();
                    if(!postback_ && importantOnly_ && feedItems.length == 0)
                        return this.getAdminFeedItems(postback_, false, gradeLevels_, start_, startDate_, endDate_, gradingPeriodId_, annType_, sortType_, toSet_);

                    model.setGradingPeriods(gradingPeriods);
                    var glsBarItemsMdl = new chlk.models.grading.GradeLevelsForTopBar(null, gradeLevels_);
                    model.setTopData(glsBarItemsMdl);
                    importantOnly_ !== undefined && model.setImportantOnly(importantOnly_);

                    return model;
                }, this);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.ClassId, Boolean]],
        function feedPrintingAction(classId_, importantOnly_) {
            var result = this.reportingService.getFeedReportSettings(classId_)
                .then(function(model){
                    model.setImportantOnly(importantOnly_ || false);
                    classId_ &&  model.setClassId(classId_);
                    return model;
                });
            return this.ShadeView(chlk.activities.feed.FeedPrintingDialog, result);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.feed.FeedPrintingViewData]],
        function submitFeedPrintingReportAction(reportViewData){
            if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
            }

            var result = this.reportingService.submitFeedReport(
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.isLessonPlanOnly(),
                    reportViewData.isIncludeAttachments(),
                    reportViewData.isIncludeDetails(),
                    reportViewData.isIncludeHiddenAttributes(),
                    reportViewData.isIncludeHiddenActivities(),
                    reportViewData.getClassId(),
                    reportViewData.isImportantOnly(),
                    reportViewData.getAnnouncementType()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.feed.FeedPrintingDialog);
                }, this)
                .thenBreak();
            return this.UpdateView(chlk.activities.feed.FeedPrintingDialog, result);
        }
    ])
});
