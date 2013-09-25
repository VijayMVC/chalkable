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

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.teacher.StudentsList');
REQUIRE('chlk.models.calendar.attendance.StudentAttendanceMonthCalendar');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');
REQUIRE('chlk.models.student.StudentProfileAttendanceViewData');
REQUIRE('chlk.models.student.StudentProfileDisciplineViewData');

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
            OVERRIDE,  ArrayOf(chlk.models.common.ActionLinkModel), function prepareActionLinksData_(user){
                var controller = 'students';
                var actionLinksData = [];
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'details', 'Now', false, [user.getId()]));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'info', 'Info', true, [user.getId()]));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'schedule', 'Schedule', false, [user.getId()]));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'attendance', 'Attendance', false, [null, user.getId()]));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'discipline', 'Discipline', false, [null, user.getId()]));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'apps', 'Apps', false, [user.getId()]));
                return actionLinksData;
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
                        var res = this.prepareUserProfileModel_(model);
                        this.getContext().getSession().set('userModel', res.getUser());
                        return res;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.StudentInfoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var result = this.studentService
                    .getSummary(personId)
                    .attach(this.validateResponse_());
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
                var schoolYearId = this.getContext().getSession().get('currentSchoolYearId');
                var res = ria.async.wait([
                            this.attendanceService.getStudentAttendanceSummary(personId, markingPeriodId_),
                            this.attendanceCalendarService.getStudentAttendancePerMonth(personId, date_),
                            this.markingPeriodService.list(schoolYearId) // todo get markingPeriods from session
                        ])
                        .attach(this.validateResponse_())
                        .then(function(result){
                            var currentMp = result[0].getMarkingPeriod();
                            var endDate = currentMp.getEndDate();
                            var startDate = currentMp.getStartDate();
                            var calendarModel = new chlk.models.calendar.attendance.StudentAttendanceMonthCalendar(date_, startDate, endDate, result[1], personId);
                            return new chlk.models.student.StudentProfileAttendanceViewData(result[0], calendarModel, result[2]);
                        });
                return this.PushView(chlk.activities.student.StudentProfileAttendancePage, res);
            },

            [[chlk.models.id.MarkingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function disciplineAction(markingPeriodId_, personId, date_){
                markingPeriodId_ = this.prepareMarkingPeriodId(markingPeriodId_);
                var schoolYearId = this.getContext().getSession().get('currentSchoolYearId');
                var res = ria.async.wait([
                        this.disciplineService.getStudentDisciplineSummary(personId, markingPeriodId_),
                        this.disciplineCalendarService.getStudentDisciplinePerMonth(personId, date_),
                        this.markingPeriodService.list(schoolYearId)  // todo get markingPeriods from session
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var currentMp = result[0].getMarkingPeriod();
                        var endDate = currentMp.getEndDate();
                        var startDate = currentMp.getStartDate();
                        var calendarModel = new chlk.models.calendar.discipline.StudentDisciplineMonthCalendar(date_, startDate, endDate, result[1], personId);
                        return new chlk.models.student.StudentProfileDisciplineViewData(result[0], calendarModel, result[2]);
                    });
                return this.PushView(chlk.activities.student.StudentProfileDisciplinePage, res);
            },

            chlk.models.id.MarkingPeriodId, function prepareMarkingPeriodId(markingPeriodId_){
                var markingPeriod = this.getContext().getSession().get('markingPeriod');
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
            }
        ])
});
