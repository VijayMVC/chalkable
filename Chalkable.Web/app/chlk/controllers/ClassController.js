REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassScheduleViewData');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');
REQUIRE('chlk.activities.classes.ClassSchedulePage');
REQUIRE('chlk.activities.classes.ClassProfileAttendancePage');
REQUIRE('chlk.activities.classes.ClassProfileGradingPage');
REQUIRE('chlk.activities.classes.ClassProfileAppsPage');
REQUIRE('chlk.activities.classes.ClassExplorerPage');
REQUIRE('chlk.activities.classes.ClassProfileDisciplinePage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ClassController */
    CLASS(
        'ClassController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.DisciplineService, 'disciplineService',

            [ria.mvc.Inject],
            chlk.services.DisciplineTypeService, 'disciplineTypeService',

            [ria.mvc.Inject],
            chlk.services.AttendanceCalendarService, 'attendanceCalendarService',

            [ria.mvc.Inject],
            chlk.services.AttendanceService, 'attendanceService',

            [ria.mvc.Inject],
            chlk.services.AnnouncementService, 'announcementService',

            [ria.mvc.Inject],
            chlk.services.GradingPeriodService, 'gradingPeriodService',

            function isPageReadonly_(clazz, teacherPermissionName, adminPermissionName){
                var teacherIds = clazz.getTeachersIds();
                var currentUserId = this.getCurrentPerson().getId();
                var permissionEnum = chlk.models.people.UserPermissionEnum;
                var isLinksEnabled = this.userIsAdmin() && this.hasUserPermission_(permissionEnum[adminPermissionName])
                    || (this.userIsTeacher() && this.hasUserPermission_(permissionEnum[teacherPermissionName])
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
                        feedModel.setReadonly(this.isPageReadonly_(model, 'VIEW_CLASSROOM', 'VIEW_CLASSROOM_ADMIN'));
                        model.setFeed(feedModel);
                        return new chlk.models.classes.ClassProfileSummaryViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);

                return this.PushView(chlk.activities.classes.SummaryPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function attendanceAction(classId){
                var res = ria.async.wait(
                    this.attendanceService.getSeatingChartInfo(classId),
                    this.classService.getAttendanceStats(classId),
                    this.classService.getAttendanceSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[2], attendances = result[0];
                        var date = new chlk.models.common.ChlkSchoolYearDate();
                        attendances.setClassId(classId);
                        attendances.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
                        attendances.setAbleChangeReasons(!this.isPageReadonly_(model, 'MAINTAIN_CLASSROOM_ABSENCE_REASONS', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN'));
                        attendances.setAblePost(!this.isPageReadonly_(model, 'MAINTAIN_CLASSROOM_ATTENDANCE', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN'));
                        attendances.setDate(date);
                        attendances.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));

                        model.setAttendances(attendances);
                        model.setStats(result[1]);
                        attendances.setInProfile(true);

                        return new chlk.models.classes.BaseClassProfileViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
                return this.PushView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, Boolean, Boolean, chlk.models.common.ChlkDate]],
            function attendanceForDateAction(classId, ableChangeReasons, ablePost, date){
                var res = this.attendanceService.getSeatingChartInfo(classId, date)
                    .attach(this.validateResponse_())
                    .then(function(attendances){
                        if(attendances){
                            attendances.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
                            attendances.setAbleChangeReasons(ableChangeReasons);
                            attendances.setAblePost(ablePost);
                            attendances.setClassId(classId);
                            attendances.setDate(date);
                            attendances.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));
                            attendances.setInProfile(true);
                        }

                        return attendances;
                    }, this);

                return this.UpdateView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.classes.DateTypeEnum, chlk.models.id.ClassId]],
            function changeAttendanceDateTypeAction(dateType, classId){
                var res = this.classService.getAttendanceStats(classId, dateType)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function disciplineAction(classId){
                var res = ria.async.wait([
                    this.disciplineService.getClassDisciplines(classId, null, 0),
                    this.disciplineTypeService.getDisciplineTypes(),
                    this.classService.getDisciplinesStats(classId),
                    this.classService.getDisciplinesSummary(classId)
                ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[3];
                        var disciplines = new chlk.models.discipline.ClassDisciplinesViewData(
                            null, classId, result[0], result[1], null, true,
                            !this.isPageReadonly_(model, 'MAINTAIN_CLASSROOM_DISCIPLINE', 'MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN'), true
                        );
                        model.setDisciplines(disciplines);
                        model.setStats(result[2]);
                        this.getContext().getSession().set(ChlkSessionConstants.DISCIPLINE_PAGE_DATA, disciplines);
                        return new chlk.models.classes.ClassProfileSummaryViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );

                    }, this);

                return this.PushView(chlk.activities.classes.ClassProfileDisciplinePage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function disciplineForDateAction(classId, date){
                var res = this.disciplineService.getClassDisciplines(classId, date, 0)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var disciplines = this.getContext().getSession().get(ChlkSessionConstants.DISCIPLINE_PAGE_DATA, null);
                        disciplines.setDisciplines(model);
                        disciplines.setDate(date);
                        this.getContext().getSession().set(ChlkSessionConstants.DISCIPLINE_PAGE_DATA, disciplines);
                        return disciplines;

                    }, this);

                return this.UpdateView(chlk.activities.classes.ClassProfileDisciplinePage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN]
            ])],
            [[chlk.models.classes.DateTypeEnum, chlk.models.id.ClassId]],
            function changeDisciplineDateTypeAction(dateType, classId){
                var res = this.classService.getDisciplinesStats(classId, dateType)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.classes.ClassProfileDisciplinePage, res);
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
