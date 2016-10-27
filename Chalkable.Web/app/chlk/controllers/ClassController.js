REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.services.SchoolYearService');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassScheduleViewData');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');
REQUIRE('chlk.activities.classes.ClassSchedulePage');
REQUIRE('chlk.activities.classes.ClassProfileAttendanceSeatingChartPage');
REQUIRE('chlk.activities.classes.ClassProfileAppsPage');
REQUIRE('chlk.activities.classes.ClassExplorerPage');
REQUIRE('chlk.activities.classes.ClassProfileDisciplinePage');
REQUIRE('chlk.activities.classes.ClassPanoramaPage');
REQUIRE('chlk.activities.classes.ClassProfileLunchPage');

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

            [ria.mvc.Inject],
            chlk.services.SchoolYearService, 'schoolYearService',

            [[chlk.models.id.ClassId]],
            function detailsAction(classId){
                var result = ria.async.wait([
                    this.classService.getSummary(classId),
                    this.announcementService.getAnnouncementsForClassProfile(classId, 0, true),
                    this.gradingPeriodService.getListByClassId(classId),
                    this.schoolYearService.listOfSchoolYearClasses(),
                    this.userIsTeacher() ? this.classService.getScheduledDays(classId) : ria.async.DeferredData([])
                ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[0], feedModel = result[1], gradingPeriods = result[2], classesByYears = result[3], classScheduledDays = result[4];
                        var teacherIds = model.getTeachersIds(), currentPersonId = this.getCurrentPerson().getId();
                        var staringEnabled = this.userIsTeacher() && teacherIds.filter(function(id){return id == currentPersonId;}).length > 0;
                        feedModel.setGradingPeriods(gradingPeriods);
                        feedModel.setClassesByYears(classesByYears);
                        feedModel.setImportantOnly(true);
                        feedModel.setInProfile(true);
                        feedModel.setClassId(classId);
                        feedModel.setStaringDisabled(!staringEnabled);
                        feedModel.setReadonly(this.isPageReadonly_('VIEW_CLASSROOM', 'VIEW_CLASSROOM_ADMIN', model));
                        classScheduledDays && feedModel.setClassScheduledDays(classScheduledDays);
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
                return this.Redirect('class', 'attendanceList', [classId]);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function gradingAction(classId){
                return this.Redirect('grading', 'summaryGridClassProfile', [classId]);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, chlk.models.classes.DateTypeEnum]],
            function attendanceListAction(classId, date_, byPostButton_){
                var dateType = this.getContext().getSession().get(ChlkSessionConstants.CHART_DATE_TYPE, chlk.models.classes.DateTypeEnum.LAST_MONTH);
                var res = ria.async.wait(
                    this.attendanceService.getClassList(classId, date_),
                    this.classService.getAttendanceStats(classId, dateType),
                    this.classService.getAttendanceSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[2], items = result[0];

                        var students = items.map(function(item){return item.getStudent()});
                        this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, students);

                        var date = date_ || new chlk.models.common.ChlkSchoolYearDate();
                        var attendances = new chlk.models.attendance.ClassList(
                            null,
                            classId,
                            items,
                            date,
                            true,
                            this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []),
                            this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE),
                            !this.isPageReadonly_('MAINTAIN_CLASSROOM_ATTENDANCE', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN', model),
                            !this.isPageReadonly_('MAINTAIN_CLASSROOM_ABSENCE_REASONS', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN', model),
                            this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM),
                            true
                        );
                        this.getContext().getSession().set(ChlkSessionConstants.CLASS_LIST_DATA, attendances);

                        model.setClassList(attendances);
                        model.setStats(result[1]);

                        var res = new chlk.models.classes.BaseClassProfileViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );

                        this.getContext().getSession().set(ChlkSessionConstants.CLASS_PROFILE_ATTENDANCE_DATA, res);

                        return res;
                    }, this);
                return this.PushOrUpdateView(chlk.activities.classes.ClassProfileAttendanceListPage, res, byPostButton_ ? 'saved' : '');
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function classListForDateAction(classId, date_){
                var res = this.attendanceService.getClassList(classId, date_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var res = this.getContext().getSession().get(ChlkSessionConstants.CLASS_LIST_DATA, null);
                        res.setItems(items);
                        date_ && res.setDate(date_);
                        return res;
                    }, this);

                return this.UpdateView(chlk.activities.classes.ClassProfileAttendanceListPage, res, '');
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
            function attendanceSeatingChartAction(classId, date_, byPostButton_){
                var dateType = this.getContext().getSession().get(ChlkSessionConstants.CHART_DATE_TYPE, chlk.models.classes.DateTypeEnum.LAST_MONTH);
                var res = ria.async.wait(
                    this.attendanceService.getSeatingChartInfo(classId, date_),
                    this.classService.getAttendanceStats(classId, dateType),
                    this.classService.getAttendanceSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[2], attendances = result[0] || new chlk.models.attendance.SeatingChart();
                        var date = date_ || new chlk.models.common.ChlkSchoolYearDate();
                        attendances.setClassId(classId);
                        attendances.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
                        attendances.setAbleChangeReasons(!this.isPageReadonly_('MAINTAIN_CLASSROOM_ABSENCE_REASONS', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN', model));
                        attendances.setAblePost(!this.isPageReadonly_('MAINTAIN_CLASSROOM_ATTENDANCE', 'MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN', model));
                        attendances.setDate(date);
                        attendances.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));

                        attendances.setInProfile(true);
                        model.setSeatingChart(attendances);
                        model.setStats(result[1]);

                        var res = new chlk.models.classes.BaseClassProfileViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );

                        this.getContext().getSession().set(ChlkSessionConstants.CLASS_PROFILE_ATTENDANCE_DATA, res);

                        return res;
                    }, this);
                return this.PushOrUpdateView(chlk.activities.classes.ClassProfileAttendanceSeatingChartPage, res, byPostButton_ ? 'saved' : '');
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function seatingChartForDateAction(classId, date_){
                var res = this.attendanceService.getSeatingChartInfo(classId, date_)
                    .attach(this.validateResponse_())
                    .then(function(attendances){
                        attendances = attendances || new chlk.models.attendance.SeatingChart();
                        var model = this.getContext().getSession().get(ChlkSessionConstants.CLASS_PROFILE_ATTENDANCE_DATA, null).getClazz().getSeatingChart();
                        date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
                        attendances.setAbleRePost(model.isAbleRePost());
                        attendances.setAbleChangeReasons(model.isAbleChangeReasons());
                        attendances.setAblePost(model.isAblePost());
                        attendances.setClassId(classId);
                        attendances.setDate(date_);
                        attendances.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));
                        attendances.setInProfile(true);

                        return attendances;
                    }, this);

                return this.UpdateView(chlk.activities.classes.ClassProfileAttendanceSeatingChartPage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
            ])],
            [[chlk.models.classes.DateTypeEnum, chlk.models.id.ClassId]],
            function changeAttendanceDateTypeAction(dateType, classId){
                this.getContext().getSession().set(ChlkSessionConstants.CHART_DATE_TYPE, dateType);
                var res = this.classService.getAttendanceStats(classId, dateType)
                    .attach(this.validateResponse_());

                return this.UpdateView(this.getView().getCurrent().getClass(), res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function disciplineAction(classId){
                var dateType = this.getContext().getSession().get(ChlkSessionConstants.CHART_DATE_TYPE, chlk.models.classes.DateTypeEnum.LAST_MONTH);
                var res = ria.async.wait([
                    this.disciplineService.getClassDisciplines(classId, null, 0),
                    this.disciplineTypeService.getDisciplineTypes(),
                    this.classService.getDisciplinesStats(classId, dateType),
                    this.classService.getDisciplinesSummary(classId)
                ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[3];
                        var disciplines = new chlk.models.discipline.ClassDisciplinesViewData(
                            null, classId, result[0], result[1], null, true,
                            !this.isPageReadonly_('MAINTAIN_CLASSROOM_DISCIPLINE', 'MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN', model), true
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

            function getPanorama_(classId, restore_){
                return ria.async.wait([
                        this.classService.getPanorama(classId)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[0];
                        var years = this.getContext().getSession().get(ChlkSessionConstants.YEARS, []);
                        model.setYears(years);
                        model.setOrderBy(chlk.models.profile.ClassPanoramaSortType.NAME);
                        restore_ && model.setShowFilters(true);
                        return new chlk.models.classes.ClassProfileSummaryViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );
                    }, this);
            },

            [[chlk.models.id.ClassId]],
            function panoramaAction(classId){
                var result = this.getPanorama_(classId);

                this.userTrackingService.viewClassPanorama();

                return this.PushView(chlk.activities.classes.ClassPanoramaPage, result);
            },

            [[chlk.models.id.ClassId]],
            function restorePanoramaAction(classId){
                var result = this.classService.restorePanorama(classId)
                    .then(function(data){
                        return this.getPanorama_(classId, true);
                    }, this);

                return this.UpdateView(chlk.activities.classes.ClassPanoramaPage, result);
            },

            [[chlk.models.profile.ClassPanoramaSortType, Boolean]],
            function sortPanoramaStudentsAction(orderBy_, descending_){
                var res = this.classService.sortPanoramaStudents(orderBy_, descending_)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.classes.ClassPanoramaPage, res, 'sort');
            },

            function panoramaSubmitAction(data){
                var filterValues = data.filterValues ? JSON.parse(data.filterValues) : '',
                    selectedStudents = data.selectedStudents,
                    highlightedStudents = data.highlightedStudents,
                    selectedAndHighlighted,
                    res, isSave = data.submitType == 'save', byCheck = data.submitType == 'check',
                    byColumn = data.submitType == 'column', isSupplemental = data.submitType == 'supplemental';

                selectedStudents = selectedStudents ? JSON.parse(selectedStudents) : '';
                highlightedStudents = highlightedStudents ? JSON.parse(highlightedStudents) : '';
                selectedAndHighlighted = selectedStudents || [];

                highlightedStudents && highlightedStudents.forEach(function(item){
                    if(selectedStudents.indexOf(item) == -1) {
                        selectedAndHighlighted.push(item);
                    }
                });

                if(isSupplemental){
                    return this.Redirect('announcement', 'supplementalAnnouncement', [data.classId, null, selectedStudents]);
                }

                if(isSave){
                    res = this.classService.savePanoramaSettings(data.classId, filterValues)
                        .attach(this.validateResponse_());
                    return this.UpdateView(chlk.activities.classes.ClassPanoramaPage, res, 'save-filters');
                }

                res = this.classService.getPanorama(data.classId, filterValues, selectedAndHighlighted)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.classes.ClassPanoramaPage, res, byCheck ? chlk.activities.lib.DontShowLoader() : '');
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

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function lunchAction(classId, date_){
                var res = ria.async.wait([
                        this.classService.getLunchCount(classId, date_ || new chlk.models.common.ChlkDate(), true),
                        this.classService.getLunchSummary(classId)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = result[1];
                        model.setLunchCountInfo(result[0]);
                        return new chlk.models.classes.ClassProfileSummaryViewData(
                            this.getCurrentRole(), model, this.getUserClaims_(),
                            this.isAssignedToClass_(classId)
                        );

                    }, this);

                return this.PushOrUpdateView(chlk.activities.classes.ClassProfileLunchPage, res);
            },

            function lunchSubmitAction(data){
                var res = this.classService.updateLunchCount(data)
                    .then(function(data){
                        this.BackgroundNavigate('class', 'lunch', [data.classId, data.date]);
                        return ria.async.BREAK;
                    }, this)

                return this.UpdateView(chlk.activities.classes.ClassProfileLunchPage, res);
            }
        ]);
});
