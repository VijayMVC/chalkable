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

REQUIRE('chlk.activities.person.ListPage');
REQUIRE('chlk.activities.student.StudentProfileSummaryPage');
REQUIRE('chlk.activities.profile.StudentInfoPage');
REQUIRE('chlk.activities.student.StudentProfileAttendancePage');
REQUIRE('chlk.activities.student.StudentProfileDisciplinePage');
REQUIRE('chlk.activities.student.StudentProfileGradingPage');
REQUIRE('chlk.activities.profile.ScheduleWeekPage');
REQUIRE('chlk.activities.profile.ScheduleMonthPage');

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
    /** @class chlk.controllers.StudentsController */
    CLASS(
        'StudentsController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

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
                        var usersModel = this.prepareUsersModel(users, 0, true);
                        var classes = this.classService.getClassesForTopBar(true);
                        var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                        return new chlk.models.teacher.StudentsList(usersModel, topModel, isMy);
                    }, this);
                return this.PushView(chlk.activities.person.ListPage, result);
            },

            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function myAction(classId_){
                return this.showStudentsList(true, classId_);
            },

            [chlk.controllers.SidebarButton('people')],
            [[chlk.models.id.ClassId]],
            function allAction(classId_){
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
                    return this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter(), rolesText);
                }, this);
                return this.UpdateView(chlk.activities.person.ListPage, result, isScroll ? chlk.activities.lib.DontShowLoader() : '');
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.studentService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var userData = this.prepareProfileData(model);
                        userData.setAbleEdit(false);
                        var res = new chlk.models.student.StudentProfileInfoViewData(this.getCurrentRole(), userData, this.getUserClaims_());
                        this.setUserToSession(res);
                        return res;
                    }, this);
                return this.PushView(chlk.activities.profile.StudentInfoPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                var res = this.infoEdit_(model, chlk.models.student.StudentProfileInfoViewData);
                return this.UpdateView(chlk.activities.profile.StudentInfoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsStudentAction(personId){
                /* Student can see ONLY his own profile CHLK-3117, CHLk-3303 */
                if (this.getCurrentPerson().getId() != personId)
                    return null;

                return this.detailsAction(personId);
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var result = this.studentService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        var role = new chlk.models.people.Role();
                        role.setId(chlk.models.common.RoleEnum.STUDENT.valueOf());
                        data.setAbleViewTranscript(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_TRANSCRIPT));
                        data.setRole(role);
                        return new chlk.models.student.StudentProfileSummaryViewData(this.getCurrentRole(), data, this.getUserClaims_());
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileSummaryPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function scheduleAction(personId){
                return this.Redirect('students', 'daySchedule', [null, personId]);
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function dayScheduleAction(date_, personId){
                return this.scheduleByRole(personId, date_
                    , chlk.models.common.RoleNamesEnum.STUDENT.valueOf()
                    , this.studentService.getSchedule(personId));
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function weekScheduleAction(date_, personId){
                var result = ria.async.wait([
                        this.personService.getSchedule(personId),
                        this.calendarService.getWeekInfo(null, date_, null, personId)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(results){
                        var schedule = results[0];
                        schedule.setRoleName(chlk.models.common.RoleNamesEnum.STUDENT.valueOf());
                        return new chlk.models.people.UserProfileScheduleViewData(
                            chlk.models.calendar.announcement.Week,
                            this.getCurrentRole(),
                            schedule,
                            results[1],
                            this.getUserClaims_()
                        );
                    }, this);
                return this.PushView(chlk.activities.profile.ScheduleWeekPage, result);
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function monthScheduleAction(date_, personId){
                var result = ria.async.wait([
                        this.personService.getSchedule(personId),
                        this.calendarService.listForMonth(null, date_, null, personId)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(results){
                        var schedule = results[0];
                        schedule.setRoleName(chlk.models.common.RoleNamesEnum.STUDENT.valueOf());
                        return new chlk.models.people.UserProfileScheduleViewData(
                            chlk.models.calendar.announcement.Month,
                            this.getCurrentRole(),
                            schedule,
                            results[1],
                            this.getUserClaims_()
                        );
                    }, this);
                return this.PushView(chlk.activities.profile.ScheduleMonthPage, result);
            },

            [[chlk.models.id.MarkingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function attendanceAction(markingPeriodId_, personId, date_){
                markingPeriodId_ = this.prepareMarkingPeriodId(markingPeriodId_);
                var res = ria.async.wait([
                            this.attendanceService.getStudentAttendanceSummary(personId, markingPeriodId_),
                            this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_),
                            this.markingPeriodService.list(this.getCurrentSchoolYearId()) // todo get markingPeriods from session
                        ])
                        .attach(this.validateResponse_())
                        .then(function(result){
                            var currentMp = result[0].getMarkingPeriod();
                            var endDate = currentMp.getEndDate();
                            var startDate = currentMp.getStartDate();
                            var calendarModel = new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(
                                date_, startDate, endDate, result[1], personId
                            );
                            return new chlk.models.student.StudentProfileAttendanceViewData(
                                this.getCurrentRole(),
                                result[0],
                                calendarModel,
                                result[2],
                                this.getUserClaims_()
                            );
                        }, this);
                return this.PushView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.id.MarkingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function disciplineAction(markingPeriodId_, personId, date_){
                markingPeriodId_ = this.prepareMarkingPeriodId(markingPeriodId_);
                var res = ria.async.wait([
                        this.disciplineService.getStudentDisciplineSummary(personId, markingPeriodId_),
                        this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_),
                        this.markingPeriodService.list(this.getCurrentSchoolYearId())  // todo get markingPeriods from session
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var currentMp = result[0].getMarkingPeriod();
                        var endDate = currentMp.getEndDate();
                        var startDate = currentMp.getStartDate();
                        var calendarModel = new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(
                            date_, startDate, endDate, result[1], personId
                        );
                        return new chlk.models.student.StudentProfileDisciplineViewData(
                            this.getCurrentRole(),
                            result[0],
                            calendarModel,
                            result[2],
                            this.getUserClaims_()
                        );
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },

            chlk.models.id.MarkingPeriodId, function prepareMarkingPeriodId(markingPeriodId_){
                var markingPeriod = this.getCurrentMarkingPeriod();
                return markingPeriodId_ && markingPeriodId_.valueOf() ? markingPeriodId_ :  markingPeriod.getId();
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function attendanceMonthAction(date_, minDate_, maxDate_, personId){
                var res = this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function(data){
                       return new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(
                           date_, minDate_, maxDate_, data, personId
                       )
                    });
                return this.UpdateView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function disciplineMonthAction(date_, minDate_, maxDate_, personId){
                var res = this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        return new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(
                            date_, minDate_, maxDate_, data, personId
                        );
                    });
                return this.UpdateView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },


            [[chlk.models.id.SchoolPersonId, chlk.models.id.MarkingPeriodId]],
            function gradingAction(studentId, markingPeriodId_){
                var currentMp = this.getCurrentMarkingPeriod();
                markingPeriodId_ = markingPeriodId_ || currentMp.getId();
                var res = ria.async.wait([
                        this.studentService.getGradingInfo(studentId, markingPeriodId_),
                        this.markingPeriodService.list(this.getCurrentSchoolYearId())
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var mp = result[1].filter(function (el){return el.getId() == markingPeriodId_})[0];
                        return new chlk.models.student.StudentProfileGradingViewData(
                            this.getCurrentRole(), result[0], mp, this.getUserClaims_()
                        );
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileGradingPage, res);
            }
        ])
});
