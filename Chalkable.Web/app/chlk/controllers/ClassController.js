REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.GradingPeriodService');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassScheduleViewData');
REQUIRE('chlk.models.classes.ClassProfileAttendanceViewData');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');
REQUIRE('chlk.activities.classes.ClassSchedulePage');
REQUIRE('chlk.activities.classes.ClassProfileAttendancePage');
REQUIRE('chlk.activities.classes.ClassProfileGradingPage');
REQUIRE('chlk.activities.classes.ClassProfileAppsPage');
REQUIRE('chlk.activities.classes.ClassExplorerPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ClassController */
    CLASS(
        'ClassController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.AttendanceCalendarService, 'attendanceCalendarService',

            [ria.mvc.Inject],
            chlk.services.AttendanceService, 'attendanceService',

            [ria.mvc.Inject],
            chlk.services.AnnouncementService, 'announcementService',

            [ria.mvc.Inject],
            chlk.services.GradingPeriodService, 'gradingPeriodService',

            function isFeedReadonly(clazz){
                var teacherIds = clazz.getTeachersIds();
                var currentUserId = this.getCurrentPerson().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var isLinksEnabled = this.userIsAdmin() && this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM_ADMIN)
                    || (this.userIsTeacher() && this.hasUserPermission_(permissionEnum.VIEW_CLASSROOM)
                    && teacherIds.filter(function(id){return id.valueOf() == currentUserId.valueOf();}).length > 0);
                return !isLinksEnabled;
            },

            [[chlk.models.id.ClassId]],
            function detailsAction(classId){
                var result = ria.async.wait([
                    this.classService.getSummary(classId),
                    this.announcementService.getAnnouncements(0, classId, true),
                    this.gradingPeriodService.getList()
                ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[0], feedModel = result[1], gradingPeriods = result[2];
                        feedModel.setGradingPeriods(gradingPeriods);
                        feedModel.setImportantOnly(true);
                        feedModel.setInProfile(true);
                        feedModel.setClassId(classId);
                        feedModel.setReadonly(this.isFeedReadonly(model));
                        model.setFeed(feedModel);
                        return new chlk.models.classes.ClassProfileSummaryViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);

                return this.PushView(chlk.activities.classes.SummaryPage, result);
            },

            [[chlk.models.id.ClassId]],
            function infoAction(classId){
                var res = this.classService
                    .getInfo(classId)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        return new chlk.models.classes.ClassProfileInfoViewData(
                            this.getCurrentRole(), data, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
                return this.PushView(chlk.activities.classes.ClassInfoPage, res);
            },

            [[chlk.models.id.ClassId]],
            function explorerAction(classId){
                var res = this.classService
                    .getExplorer(classId)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        data.setClaims(this.getUserClaims_());
                        data.setCurrentRoleId(this.getCurrentRole().getRoleId());
                        return data;
                    }, this);
                return this.PushView(chlk.activities.classes.ClassExplorerPage, res);
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function scheduleAction(classId, date_){
                var res = this.schedule_(classId, date_);
                return this.PushOrUpdateView(chlk.activities.classes.ClassSchedulePage, res);
            },
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function scheduleUpdateAction(classId, date){
                var res = this
                        .schedule_(classId, date)
                        .then(function(data){
                            return data.getScheduleCalendar();
                        });
                return this.UpdateView(chlk.activities.classes.ClassSchedulePage, res);
            },
            function schedule_(classId, date_){
                var mp = this.getCurrentMarkingPeriod();
                return this.classService
                    .getSchedule(classId, date_)
                    .attach(this.validateResponse_())
                    .then(function (data){

                        var scheduleCalendar = this.getContext().getService(chlk.services.CalendarService)
                            .prepareWeekData(data.getCalendarDayItems(), date_);

                        return new chlk.models.classes.ClassScheduleViewData(
                            this.getCurrentRole(),
                            data.getClazz(),
                            scheduleCalendar,
                            this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
            },
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function attendanceAction(classId, date_){
                var res = ria.async.wait(
                        this.classService.getAttendance(classId),
                        this.attendanceCalendarService.getClassAttendancePerMonth(classId, date_)
                    )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var period;
                        if(this.userIsAdmin()){
                            period = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR);
                        }else{
                            period = this.getCurrentGradingPeriod();
                        }
                        var attCalendar = new chlk.models.calendar.attendance.ClassAttendanceMonthCalendar(
                            date_, period.getStartDate(), period.getEndDate(), result[1], classId
                        );
                        return new chlk.models.classes.ClassProfileAttendanceViewData(
                            this.getCurrentRole(), result[0],
                            attCalendar, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
                return this.PushView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function gradingAction(classId){
                var res =
                    this.classService
                        .getGrading(classId)
                        .attach(this.validateResponse_())
                        .then(function(model){
                            model.getGradingPerMp().forEach(function(mpData){
                                mpData.getByAnnouncementTypes().forEach(function(item){
                                    item.setClassId(classId);
                                });
                            });
                            return new chlk.models.classes.ClassProfileGradingViewData(
                                this.getCurrentRole(),
                                model, this.getUserClaims_(),
                                this.isAssignedToClass_(classId)
                            );
                        }, this);
                return this.PushView(chlk.activities.classes.ClassProfileGradingPage, res);
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function attendanceMonthAction(classId, date){
                var res =  this.attendanceCalendarService
                    .getClassAttendancePerMonth(classId, date)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var period;
                        if(this.userIsAdmin()){
                            period = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR);
                        }else{
                            period = this.getCurrentGradingPeriod();
                        }
                        return new chlk.models.calendar.attendance.ClassAttendanceMonthCalendar(
                            date, period.getStartDate(), period.getEndDate(), data, classId
                        );
                    }, this);
                return this.UpdateView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },

            [[chlk.models.id.ClassId]],
            function appsAction(classId){
                var res = this.classService.getAppsInfo(classId)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return new chlk.models.classes.ClassProfileAppsViewData(
                            this.getCurrentRole(),
                            data, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
                return this.PushView(chlk.activities.classes.ClassProfileAppsPage, res);
            },

            [[chlk.models.id.ClassId]],
            Boolean, function isAssignedToClass_(classId){
               return  !!this.classService.getClassById(classId);
            }
        ]);
});
