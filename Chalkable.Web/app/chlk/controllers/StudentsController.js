REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');
REQUIRE('chlk.services.AttendanceService');
REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineCalendarService');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.SchoolYearService');

REQUIRE('chlk.activities.person.ListPage');
REQUIRE('chlk.activities.student.StudentProfileSummaryPage');
REQUIRE('chlk.activities.profile.StudentInfoPage');
REQUIRE('chlk.activities.student.StudentProfileAttendancePage');
REQUIRE('chlk.activities.student.StudentProfileDisciplinePage');
REQUIRE('chlk.activities.student.StudentProfileAssessmentPage');
REQUIRE('chlk.activities.student.StudentProfileGradingPage');
REQUIRE('chlk.activities.profile.ScheduleWeekPage');
REQUIRE('chlk.activities.profile.ScheduleMonthPage');
REQUIRE('chlk.activities.student.StudentProfileExplorerPage');
REQUIRE('chlk.activities.attendance.StudentDayAttendancePopup');
REQUIRE('chlk.activities.discipline.StudentDayDisciplinePopup');
REQUIRE('chlk.activities.student.StudentProfileGradingPopup');
REQUIRE('chlk.activities.student.StudentProfilePanoramaPage');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.teacher.StudentsList');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');
REQUIRE('chlk.models.student.StudentProfileAttendanceViewData');
REQUIRE('chlk.models.student.StudentProfileDisciplineViewData');
REQUIRE('chlk.models.student.StudentProfileSummaryViewData');
REQUIRE('chlk.models.student.StudentProfileInfoViewData');
REQUIRE('chlk.models.student.StudentProfileGradingViewData');

NAMESPACE('chlk.controllers', function (){
    "use strict";

    var listFutures = [];

    /** @class chlk.controllers.StudentsController */
    CLASS(
        'StudentsController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.AttendanceCalendarService, 'attendanceCalendarService',

            [ria.mvc.Inject],
            chlk.services.AttendanceService, 'attendanceService',

            [ria.mvc.Inject],
            chlk.services.MarkingPeriodService, 'markingPeriodService',

            [ria.mvc.Inject],
            chlk.services.DisciplineService, 'disciplineService',

            [ria.mvc.Inject],
            chlk.services.DisciplineCalendarService, 'disciplineCalendarService',

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [ria.mvc.Inject],
            chlk.services.SchoolYearService, 'schoolYearService',

            [chlk.controllers.SidebarButton('people')],
            function indexStudentAction() {
                var classId = this.getCurrentClassId();
                return this.Redirect('students', 'my', [classId]);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_STUDENT, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_STUDENTS]
            ])],
            [chlk.controllers.SidebarButton('people')],
            function indexTeacherAction() {
                var classId = this.getCurrentClassId();
                return this.Redirect('students', 'my', [classId]);
            },

            function getInfoPageClass(){
                return chlk.activities.profile.StudentInfoPage;
            },

            //TODO: refactor
            [chlk.controllers.SidebarButton('people')],
            [[Boolean, chlk.models.id.ClassId]],
            function showStudentsList(isMy, classId_){
                var result, isStudent = this.getCurrentRole().isStudent();
                if(isStudent && !isMy){
                    result = this.teacherService
                        .getTeachers(classId_, null, true, 0, 10, true)
                        .attach(this.validateResponse_());
                }else{
                    result = this.studentService
                        .getStudents(classId_, null, !isStudent && isMy, true, 0, 10)
                        .attach(this.validateResponse_());
                }
                result = result.then(function(users){
                        var usersModel = this.prepareUsersModel(users, 0, true, null, null, isMy);
                        var topModel = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        return new chlk.models.teacher.StudentsList(usersModel, topModel, isMy, null, this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS),
                            this.getCurrentRole().isStudent() || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_STUDENT));
                    }, this);
                return this.PushView(chlk.activities.person.ListPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_STUDENT, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_STUDENTS]
            ])],
            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function myTeacherAction(classId_){
                return this.showStudentsList(true, classId_);
            },

            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function myStudentAction(classId_){
                return this.showStudentsList(true, classId_);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_STUDENT]
            ])],
            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function allTeacherAction(classId_){
                return this.showStudentsList(false, classId_);
            },

            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function allStudentAction(classId_){
                return this.showStudentsList(false, classId_);
            },

            //TODO: refactor
            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.teacher.StudentsList]],
            function updateListAction(model){
                var isScroll = model.isScroll(),
                    start = model.getStart();
                var result, isStudent = this.getCurrentRole().isStudent(), rolesText;
                if(isStudent && !model.isMy()){
                    rolesText = Msg.Teacher(users.getTotalCount()!=1);
                    result = this.teacherService
                        .getTeachers(
                            model.getClassId(),
                            model.getFilter(),
                            model.isByLastName(),
                            start,
                            model.getCount(),
                            true
                        )
                        .attach(this.validateResponse_());
                }else{
                    rolesText = Msg.Student(users.getTotalCount()!=1);
                    result = this.studentService
                        .getStudents(
                            model.getClassId(),
                            model.getFilter(),
                            !isStudent && model.isMy(),
                            model.isByLastName(),
                            start,
                            model.getCount()
                        )
                        .attach(this.validateResponse_());
                }

                result = result.then(function(usersData){
                    if(isScroll)  return this.prepareUsers(usersData, start);
                    return this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter(), rolesText, model.isMy());
                }, this);

                if(isScroll){
                    listFutures.push(result);
                }else{
                    listFutures.forEach(function(item){
                        item.cancel();
                    });
                    listFutures = [];
                }

                return this.UpdateView(chlk.activities.person.ListPage, result, isScroll ? chlk.activities.lib.DontShowLoader() : '');
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.studentService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var userData = this.prepareProfileData(model);
                        var res = new chlk.models.student.StudentProfileInfoViewData(this.getCurrentRole(), userData, this.getUserClaims_());
                        this.setUserToSession(res);
                        return res;
                    }, this);
                return this.PushView(chlk.activities.profile.StudentInfoPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                var result = this.infoEdit_(model, chlk.models.student.StudentProfileInfoViewData);
                return this.UpdateView(chlk.activities.profile.StudentInfoPage, result);
            },

            function getPanorama_(personId, restore_){
                return ria.async.wait([
                        this.studentService.getInfoForPanorama(personId),
                        this.studentService.getPanorama(personId),
                        this.schoolYearService.list()
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var userData = result[0];
                        var panorama = result[1];
                        panorama.setSchoolYears(result[2]);
                        panorama.setShowFilters(restore_ || false);
                        userData.setPanoramaInfo(panorama);
                        var res = new chlk.models.student.StudentProfilePanoramaViewData(this.getCurrentRole(), userData, this.getUserClaims_());
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_PANORAMA, userData);
                        this.setUserToSession(res);
                        return res;
                    }, this);
            },

            [[chlk.models.id.SchoolPersonId]],
            function panoramaAction(personId){
                var result = this.getPanorama_(personId);

                this.userTrackingService.viewStudentPanorama();

                return this.PushView(chlk.activities.student.StudentProfilePanoramaPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function restorePanoramaAction(personId){
                var result = this.studentService.restorePanorama()
                    .then(function(data){
                        return this.getPanorama_(personId, true);
                    }, this);

                return this.UpdateView(chlk.activities.student.StudentProfilePanoramaPage, result);
            },

            function panoramaSubmitAction(data){
                var filterValues = data.filterValues ? JSON.parse(data.filterValues) : '',
                    res, isSave = data.submitType == 'save';

                if(isSave){
                    res = this.studentService.savePanoramaSettings(data.studentId, filterValues)
                        .attach(this.validateResponse_());
                    return this.UpdateView(chlk.activities.student.StudentProfilePanoramaPage, res, 'save-filters');
                }

                res = this.studentService.getPanorama(data.studentId, filterValues)
                    .then(function(panorama){
                        var userData = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PANORAMA, null);
                        var schoolYears = userData.getPanoramaInfo().getSchoolYears();
                        panorama.setSchoolYears(schoolYears);
                        userData.setPanoramaInfo(panorama);
                        return userData;
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.student.StudentProfilePanoramaPage, res);
            },

            [[chlk.models.panorama.StudentAttendancesSortType, Boolean]],
            function sortPanoramaAttendancesAction(orderBy_, descending_){
                var res = this.studentService.sortPanoramaAttendances(orderBy_, descending_)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.student.StudentProfilePanoramaPage, res, 'sort-attendances');
            },

            [[chlk.models.panorama.StudentDisciplinesSortType, Boolean]],
            function sortPanoramaDisciplinesAction(orderBy_, descending_){
                var res = this.studentService.sortPanoramaDisciplines(orderBy_, descending_)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.student.StudentProfilePanoramaPage, res, 'sort-disciplines');
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsStudentAction(personId){
                /* Student can see ONLY his own profile CHLK-3117, CHLk-3303 */
                if (this.getCurrentPerson().getId() != personId)
                    return null;

                return this.detailsAction(personId);
            },



        //move it somewhere
        [[String, chlk.models.id.SchoolPersonId, String]],
        ria.async.Future, function prepareExternalAttachAppViewDataForStudent_(mode, studentId, appUrlAppend_){

            var appId = chlk.models.id.AppId(_GLOBAL.assessmentApplicationId);

            return this.appsService
                .getAccessToken(this.getCurrentPerson().getId(), null, appId)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                })
                .attach(this.validateResponse_())
                .then(function(data){
                    var appData = data.getApplication();

                    var viewUrl = appData.getUrl() + '?mode=' + mode
                        + '&apiRoot=' + encodeURIComponent(_GLOBAL.location.origin)
                        + '&token=' + encodeURIComponent(data.getToken())
                        + '&studentId=' + studentId.valueOf()
                        + (appUrlAppend_ ? '&' + appUrlAppend_ : '');

                    return new chlk.models.apps.ExternalAttachAppViewData(null, appData, viewUrl, '');
                }, this);
            },

            [chlk.controllers.AssessmentEnabled()],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DISTRICTADMIN,
                chlk.models.common.RoleEnum.TEACHER,
                chlk.models.common.RoleEnum.STUDENT
            ])],
            [chlk.controllers.SidebarButton('settings')],
            [[chlk.models.id.SchoolPersonId]],
            function assessmentProfileAction(personId) {
                
                var result = ria.async.wait([
                    this.prepareExternalAttachAppViewDataForStudent_("profileview", personId),
                    this.prepareStudentProfileSummaryViewData_(personId, chlk.models.student.StudentProfileAssessmentViewData)
                ])
                .attach(this.validateResponse_())
                .then(function(data){
                    var externalAttachViewData = data[0];
                    var summaryViewData = data[1];
                    summaryViewData.setAttachAppViewData(externalAttachViewData);
                    return summaryViewData;
                });
                return this.PushView(chlk.activities.student.StudentProfileAssessmentPage, result);
            },     


            [[chlk.models.id.SchoolPersonId, Function]],
            function prepareStudentProfileSummaryViewData_(personId, mdl){
                return this.studentService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        var role = new chlk.models.people.Role();
                        role.setId(chlk.models.common.RoleEnum.STUDENT.valueOf());
                        data.setAbleViewTranscript(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_TRANSCRIPT));
                        if(!this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE) &&
                            !this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE_ADMIN))
                                data.setDisciplineBox(null);
                        if(!this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE) &&
                            !this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN))
                                data.setAttendanceBox(null);
                        data.setRole(role);
                        return new mdl(this.getCurrentRole(), data, this.getUserClaims_());
                    }, this);
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var result = this.prepareStudentProfileSummaryViewData_(personId, chlk.models.student.StudentProfileSummaryViewData);
                return this.PushView(chlk.activities.student.StudentProfileSummaryPage, result);
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function attendanceAction(gradingPeriodId_, personId, date_){
                gradingPeriodId_ = this.prepareGradingPeriodId(gradingPeriodId_);
                var res = ria.async.wait([
                        this.studentService.getStudentAttendanceSummary(personId, gradingPeriodId_),
                        this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var currentGp = result[0].getCurrentGradingPeriod();
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_GP, currentGp);
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS, result[1]);
                        var calendarModel = new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(
                            date_, result[1], personId, currentGp
                        );
                        return new chlk.models.student.StudentProfileAttendanceViewData(
                            this.getCurrentRole(),
                            result[0],
                            calendarModel,
                            this.getUserClaims_()
                        );
                    }, this);
                return this.PushOrUpdateView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function disciplineAction(gradingPeriodId_, personId, date_){
                gradingPeriodId_ = this.prepareGradingPeriodId(gradingPeriodId_);
                var res = ria.async.wait([
                        this.studentService.getStudentDisciplineSummary(personId, gradingPeriodId_),
                        this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var currentGp = result[0].getCurrentGradingPeriod();
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_GP, currentGp);
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS, result[1]);
                        var calendarModel = new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(
                            date_, result[1], personId, currentGp
                        );
                        return new chlk.models.student.StudentProfileDisciplineViewData(
                            this.getCurrentRole(),
                            result[0],
                            calendarModel,
                            this.getUserClaims_()
                        );
                    }, this);
                return this.PushOrUpdateView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String]],
            function showStudentAttendanceAction(studentId, date, controller_, action_, params_) {
                var items = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS);
                var item = items.filter(function(day){
                    return day.getDate().isSameDay(date);
                })[0];
                var target = chlk.controls.getActionLinkControlLastNode(),
                    reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []),
                    canRePost = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE),
                    canSetAttendance = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
                        || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN),
                    canChangeReasons = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS)
                        || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN),
                    model = new chlk.models.attendance.StudentDayAttendances(target, item, reasons, canRePost, canSetAttendance,
                        canChangeReasons, controller_, action_, params_);
                return this.ShadeView(chlk.activities.attendance.StudentDayAttendancePopup, new ria.async.DeferredData(model));
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String]],
            function showStudentDisciplineAction(studentId, date, controller_, action_, params_) {
                var items = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS);
                var item = items.filter(function(day){
                    return day.getDate().isSameDay(date);
                })[0];
                var target = chlk.controls.getActionLinkControlLastNode(),
                    canSetDiscipline = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE)
                        || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN),
                    model = new chlk.models.discipline.StudentDayDisciplines(target, item, canSetDiscipline, controller_, action_, params_);
                return this.ShadeView(chlk.activities.discipline.StudentDayDisciplinePopup, new ria.async.DeferredData(model));
            },

            chlk.models.id.GradingPeriodId, function prepareGradingPeriodId(gradingPeriodId_){
                var gardingPeriod = this.getCurrentGradingPeriod();
                return gradingPeriodId_ && gradingPeriodId_.valueOf() ? gradingPeriodId_ :  gardingPeriod.getId();
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function attendanceMonthAction(minDate_, maxDate_, personId, date_){
                var res = this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var currentGP = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CALENDAR_GP, null);
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS, data);
                        return new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(
                           date_, data, personId, currentGP
                        )
                    }, this);
                return this.UpdateView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function disciplineMonthAction(minDate_, maxDate_, personId, date_){
                var res = this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        var currentGP = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CALENDAR_GP, null);
                        this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CALENDAR_ITEMS, data);
                        return new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(
                            date_, data, personId, currentGP
                        );
                    }, this);
                return this.UpdateView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },


            [[chlk.models.id.SchoolPersonId]],
            function gradingAction(studentId){
                var gradingPeriod = this.getCurrentGradingPeriod();
                var res = this.studentService.getGradingInfo(studentId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return new chlk.models.student.StudentProfileGradingViewData(
                            this.getCurrentRole(), model, gradingPeriod, this.getUserClaims_()
                        );
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileGradingPage, res);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId]],
            function loadGradingDetailsAction(studentId, gradingPeriodId){
                var res = this.studentService.getGradingDetailsForPeriod(studentId, gradingPeriodId)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.student.StudentProfileGradingPage, res);
            },

            [[Number, chlk.models.id.ClassId]],
            function showGradingActivitiesForStudentAction(announcementTypeId, classId){
                var model = this.studentService.getGradingActivitiesForStudent(announcementTypeId, classId);
                model.setTarget(chlk.controls.getActionLinkControlLastNode());
                return this.ShadeView(chlk.activities.student.StudentProfileGradingPopup, ria.async.DeferredData(model));
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function dayScheduleAction(personId, date_){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.STUDENT.valueOf(),
                    personId,
                    date_,
                    this.calendarService.getDayWeekInfo(date_, personId),
                    chlk.models.calendar.announcement.Week,
                    chlk.activities.profile.SchedulePage,
                    'daySchedule',
                    chlk.templates.calendar.announcement.DayCalendarBodyTpl
                );
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function weekScheduleAction(personId, date_){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.STUDENT.valueOf(),
                    personId,
                    date_,
                    this.calendarService.getDayWeekInfo(date_, personId),
                    chlk.models.calendar.announcement.Week,
                    chlk.activities.profile.ScheduleWeekPage,
                    'weekSchedule',
                    chlk.templates.calendar.announcement.WeekCalendarBodyTpl
                );
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function monthScheduleAction(personId, date_){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.STUDENT.valueOf(),
                    personId,
                    date_,
                    this.calendarService.listForMonth(null, date_, null, personId),
                    chlk.models.calendar.announcement.Month,
                    chlk.activities.profile.ScheduleMonthPage,
                    'monthSchedule',
                    chlk.templates.calendar.announcement.MonthCalendarBodyTpl
                );
            },

            [chlk.controllers.StudyCenterEnabled()],
            [[chlk.models.id.SchoolPersonId]],
            function explorerAction(personId){
                var res = this.studentService.getExplorer(personId)
                    .attach(this.validateResponse_())
                    .then(function(studentExplorer){
                        return new chlk.models.student.StudentExplorerViewData(
                            this.getCurrentRole(), studentExplorer, this.getUserClaims_()
                        )
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileExplorerPage, res);
            },

            [[chlk.models.id.SchoolPersonId]],
            function appsAction(personId) {
                var res = this.studentService
                    .getAppsInfo(personId, 0, 10000)
                    .attach(this.validateResponse_())
                    .then(function (model) {
                        return new chlk.models.people.UserProfileAppsViewData(this.getCurrentRole(), model, this.getUserClaims_());
                    }, this);
                return this.PushView(chlk.activities.profile.SchoolPersonAppsPage, res);
            }
        ])
});
