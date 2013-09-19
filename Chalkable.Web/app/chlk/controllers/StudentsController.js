REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.person.ListPage');
REQUIRE('chlk.activities.student.SummaryPage');
REQUIRE('chlk.activities.profile.StudentInfoPage');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.teacher.StudentsList');

NAMESPACE('chlk.controllers', function (){
    "use strict";
    /** @class chlk.controllers.StudentsController */
    CLASS(
        'StudentsController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',



            //TODO: refactor
            OVERRIDE,  ArrayOf(chlk.models.common.ActionLinkModel), function prepareActionLinksData_(){
                var controller = 'students';
                var actionLinksData = [];
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'details', 'Now'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'info', 'Info', true));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'schedule', 'Schedule'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'attendance', 'Attendance'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'discipline', 'Discipline'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'apps', 'Apps'));
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
            }
        ])
});
