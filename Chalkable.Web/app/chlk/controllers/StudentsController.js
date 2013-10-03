REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');
REQUIRE('chlk.services.AttendanceService');
REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineCalendarService');

REQUIRE('chlk.activities.person.ListPage');
REQUIRE('chlk.activities.student.SummaryPage');
REQUIRE('chlk.activities.profile.StudentInfoPage');
REQUIRE('chlk.activities.student.StudentProfileAttendancePage');
REQUIRE('chlk.activities.student.StudentProfileDisciplinePage');
REQUIRE('chlk.activities.student.StudentProfileGradingPage');

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
            [[Boolean, chlk.models.id.ClassId]],
            function showStudentsList(isMy, classId_){
                var result = this.studentService.getStudents(classId_, null, isMy, true, 0, 10)
                    .then(function(users){
                        var usersModel = this.prepareUsersModel(users, 0, true);
                        var classes = this.classService.getClassesForTopBar(true);
                        var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                        return new chlk.models.teacher.StudentsList(usersModel, topModel, isMy);
                    }.bind(this));
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
            [[chlk.models.teacher.StudentsList]],
            function updateListAction(model){
                var isScroll = model.isScroll(), start = model.getStart();
                var result = this.studentService.getStudents(model.getClassId(), model.getFilter(), model.isMy(), model.isByLastName(), start, 10)
                    .then(function(usersData){
                        if(isScroll)  return this.prepareUsers(usersData, start);
                        return this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter());
                    }.bind(this));
                return this.UpdateView(chlk.activities.person.ListPage, result, isScroll ? chlk.activities.lib.DontShowLoader() : '');
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.studentService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var userData = this.prepareProfileData(model);
                        var res = new chlk.models.student.StudentProfileInfoViewData(this.getCurrentRole(), userData);
                        this.setUserToSession(res);
                        return res;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.StudentInfoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var result = this.studentService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        var role = new chlk.models.people.Role();
                        role.setId(chlk.models.common.RoleEnum.STUDENT.valueOf());
                        data.setRole(role);
                        return new chlk.models.student.StudentProfileSummaryViewData(this.getCurrentRole(), data);
                    }, this);
                return this.PushView(chlk.activities.student.SummaryPage, result);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function scheduleAction(personId, date_){
                //return BASE(personId, chlk.models.common.RoleNamesEnum.TEACHER);
                return this.scheduleByRole(personId, date_, chlk.models.common.RoleNamesEnum.STUDENT.valueOf());
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
                            var calendarModel = new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(date_, startDate, endDate, result[1], personId);
                            return new chlk.models.student.StudentProfileAttendanceViewData(this.getCurrentRole(), result[0], calendarModel, result[2]);
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
                        var calendarModel = new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(date_, startDate, endDate, result[1], personId);
                        return new chlk.models.student.StudentProfileDisciplineViewData(this.getCurrentRole(), result[0], calendarModel, result[2]);
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },

            chlk.models.id.MarkingPeriodId, function prepareMarkingPeriodId(markingPeriodId_){
                var markingPeriod = this.getCurrentMarkingPeriod();
                return markingPeriodId_ || markingPeriod.getId();
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function attendanceMonthAction(date_, minDate_, maxDate_, personId){
                var res = this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(date_, minDate_, maxDate_, data, personId)
                    });
                return this.UpdateView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function disciplineMonthAction(date_, minDate_, maxDate_, personId){
                var res = this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        return new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(date_, minDate_, maxDate_, data, personId);
                    });
                return this.UpdateView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },


            [[chlk.models.id.SchoolPersonId, chlk.models.id.MarkingPeriodId]],
            function gradingAction(studentId, markingPeriodId_){
                var mp = this.getCurrentMarkingPeriod();
                markingPeriodId_ = markingPeriodId_ || mp.getId();
                var res = this.studentService.getGradingInfo(studentId, markingPeriodId_)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return new chlk.models.student.StudentProfileGradingViewData(this.getCurrentRole(), data);
                    }, this);
                return this.PushView(chlk.activities.student.StudentProfileGradingPage, res);
            }
        ])
});
