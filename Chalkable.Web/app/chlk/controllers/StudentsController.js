REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.person.ListPage');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.teacher.StudentsList');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.StudentsController */
    CLASS(
        'StudentsController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [[chlk.models.common.PaginatedList, Number, Boolean, String]],
            chlk.models.people.UsersList, function prepareUsersModel(users, selectedIndex, byLastName, filter_){
                users = this.prepareUsers(users);
                var usersModel = new chlk.models.people.UsersList();
                usersModel.setSelectedIndex(selectedIndex);
                usersModel.setByLastName(byLastName);
                filter_ && usersModel.setFilter(filter_);
                usersModel.setUsers(users);
                return usersModel;
            },

            [[chlk.models.common.PaginatedList, Number]],
            chlk.models.common.PaginatedList, function prepareUsers(usersData, start_){
                var start = start_ || 0;
                usersData.getItems().forEach(function(item, index){
                    item.setIndex(start_ + index);
                    item.setPictureUrl(this.personService.getPictureURL(item.getId(), 47));
                }.bind(this));
                return usersData;
            },

            [[Boolean, chlk.models.id.ClassId]],
            function showStudentsList(isMy, classId_){
                var result = this.studentService.getStudents(classId_, null, isMy, true, 0, 10)
                    .then(function(users){
                        var model = new chlk.models.teacher.StudentsList();
                        var usersModel = this.prepareUsersModel(users, 0, true);
                        var classes = this.classService.getClassesForTopBar(true);
                        model.setUsersPart(usersModel);
                        model.setMy(isMy);
                        var topModel = new chlk.models.class.ClassesForTopBar();
                        topModel.setTopItems(classes);
                        topModel.setDisabled(false);
                        classId_ && topModel.setSelectedItemId(classId_);
                        model.setTopData(topModel);
                        return model;
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

            [[chlk.models.teacher.StudentsList]],
            function updateListAction(model){
                var submitType = model.getSubmitType();
                var isScroll = submitType == "scroll", start = model.getStart();
                var result = this.studentService.getStudents(model.getClassId(), model.getFilter(), model.isMy(), model.isByLastName(), start, 10)
                    .then(function(usersData){
                        if(isScroll){
                            usersData = this.prepareUsers(usersData, start);
                            return usersData;
                        }else{
                            var usersModel = this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter());
                            return usersModel;
                        }

                    }.bind(this));
                return this.UpdateView(chlk.activities.person.ListPage, result, isScroll ? window.noLoadingMsg : '');
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.studentService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var res = this.prepareProfileData(model);
                        this.getContext().getSession().set('userModel', res);
                        return res;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.InfoViewPage, result);
            }
        ])
});
