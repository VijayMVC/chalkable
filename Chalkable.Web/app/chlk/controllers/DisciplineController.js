REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradeLevelService');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineList');
REQUIRE('chlk.models.discipline.DisciplineInputModel');
REQUIRE('chlk.models.discipline.SetDisciplineListModel');
REQUIRE('chlk.models.discipline.PaginatedListByDateModel');
REQUIRE('chlk.models.attendance.AttendanceStudentBox');

REQUIRE('chlk.activities.discipline.DisciplineSummaryPage');
REQUIRE('chlk.activities.discipline.AdminDisciplineSummaryPage');
REQUIRE('chlk.activities.discipline.SetDisciplineDialog');
REQUIRE('chlk.activities.discipline.DayDisciplinePopup');
REQUIRE('chlk.activities.discipline.DisciplineDayPopup');
REQUIRE('chlk.activities.discipline.ClassDisciplinesPage');

NAMESPACE('chlk.controllers', function(){
    "use strict";
    /** @class chlk.controllers.DisciplineController */

    CLASS(
        'DisciplineController', EXTENDS(chlk.controllers.BaseController),[

            [ria.mvc.Inject],
            chlk.services.DisciplineService, 'disciplineService',

            [ria.mvc.Inject],
            chlk.services.DisciplineTypeService, 'disciplineTypeService',

            [ria.mvc.Inject],
            chlk.services.GradeLevelService, 'gradeLevelService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',


            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE
            ])],
            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.common.ChlkDate, Number, Number]],
            function listAction(date_, pageSize_, pageIndex_){
                var res = this.disciplineList_(pageIndex_, pageSize_, date_);
                return this.PushView(chlk.activities.discipline.DisciplineSummaryPage, res);
            },

            [chlk.controllers.SidebarButton('discipline')],
            [[Boolean, String, chlk.models.common.ChlkDate]],
            function listAdminAction(update_, gradeLevelsIds_, date_){
                var res = this.disciplineService
                    .summary(date_, gradeLevelsIds_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
                        var topModel = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevelsIds_);
                        model.setTopData(topModel);
                        if(gradeLevelsIds_)
                            model.setGradeLevelsIds(gradeLevelsIds_);
                        return model;
                    }, this);
                return update_ ? this.UpdateView(chlk.activities.discipline.AdminDisciplineSummaryPage, res) :
                    this.PushView(chlk.activities.discipline.AdminDisciplineSummaryPage, res);
            },

            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.common.ChlkDate, Number, Number, Boolean]],
            function pageAction(date_, pageSize, pageIndex, byLastName_){
                var res = this.disciplineList_(pageIndex, pageSize, date_);
                return this.UpdateView(chlk.activities.discipline.DisciplineSummaryPage, res);
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_DISCIPLINE
            ])],
            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function classListAction(classId_, date_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('discipline', 'list', [date_]);
                var res = ria.async.wait([
                        this.disciplineService.getClassDisciplines(classId_, date_, 0),
                        this.disciplineTypeService.getDisciplineTypes()
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var classes = this.getClassForDisciplines_();
                        var classBarData = new chlk.models.classes.ClassesForTopBar(classes);

                        var model = new chlk.models.discipline.ClassDisciplinesViewData(
                            classBarData, classId_, result[0], result[1], date_, true,
                            this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE) ||
                                this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_DISCIPLINE_ADMIN)
                        );
                        this.getContext().getSession().set(ChlkSessionConstants.DISCIPLINE_PAGE_DATA, model);
                        return model;

                      }, this);

                var activityClass = chlk.activities.discipline.ClassDisciplinesPage;
                var currentActivity = this.getView().getCurrent();
                if(currentActivity && activityClass == currentActivity.getClass()){
                    return this.UpdateView(activityClass, res);
                }
                return this.PushView(activityClass, res);
            },

            //TODO: refactor this copy past
            [[Boolean]],
            function sortStudentsAction(byLastName){
                var model = this.getContext().getSession().get(ChlkSessionConstants.DISCIPLINE_PAGE_DATA);
                model.getDisciplines().sort(function(item1, item2){
                    var sortField1 = "";
                    var sortField2 = "";

                    if (byLastName){
                        sortField1 = item1.getStudent().getLastName();
                        sortField2 = item2.getStudent().getLastName();
                    }
                    else{
                        sortField1 = item1.getStudent().getFirstName();
                        sortField2 = item2.getStudent().getFirstName();
                    }
                    return strcmp(sortField1, sortField2);
                });
                model.setByLastName(byLastName);
                var result = new ria.async.DeferredData(model);
                return this.UpdateView(chlk.activities.discipline.ClassDisciplinesPage, result);
            },

            [[Number, Number, chlk.models.common.ChlkDate]],
            ria.async.Future, function disciplineList_(pageIndex_, pageSize_, date_){
                var start = 0;
                if(pageIndex_ && pageSize_)
                    start = pageIndex_ * pageSize_;
                return this.disciplineService
                    .list(date_, start)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
                        var classes = this.getClassForDisciplines_();
                        var classBarData = new chlk.models.classes.ClassesForTopBar(classes);
                        return new ria.async.DeferredData(new chlk.models.discipline.PaginatedListByDateModel(classBarData, data, date_));
                    }, this);
            },

            Array, function getClassForDisciplines_(){
                return this.classService.getClassesForTopBar();
            },

            [[chlk.models.discipline.DisciplineInputModel]],
            function listStudentDisciplineAction(model){
                var res = this.listStudentDiscipline_(model.getPersonId(), model.getDisciplineDate());
                return this.ShadeView(chlk.activities.discipline.SetDisciplineDialog, res);
            },
            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String, Boolean]],
            function showStudentDayDisciplinesAction(studentId, date, controller_, action_, params_, isNew_){
                var res = this.listStudentDiscipline_(studentId, date)
                    .then(function(model){
                        model.setTarget(chlk.controls.getActionLinkControlLastNode());
                        model.setAbleEdit(this.userIsAdmin() || this.userInRole(chlk.models.common.RoleEnum.TEACHER));
                        if(controller_)
                            model.setController(controller_);
                        if(action_)
                            model.setAction(action_);
                        if(params_)
                            model.setParams(params_);
                        if(isNew_)
                            model.setNewStudent(true);
                        return model;
                    }, this);
                return this.ShadeView(chlk.activities.discipline.DayDisciplinePopup, res);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listStudentDiscipline_(studentId, date){
                //todo move disciplineTypes to session
                return ria.async.wait([
                        this.disciplineTypeService.getDisciplineTypes(),
                        this.disciplineService.listStudentDiscipline(null, studentId, date)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        date = date || new chlk.models.common.ChlkSchoolYearDate();
                        var model = result[1];
                        model.setDisciplineTypes(result[0]);
                        model.setDate(date);
                        return model;
                    }, this);
            },

            [[chlk.models.discipline.SetDisciplineListModel]],
            function setDisciplinesForListAction(model){
                this.disciplineService
                    .setDisciplines(model)
                    .attach(this.validateResponse_())
                        .then(function(data){
                            var controller = model.getController();
                            if(controller){
                                var action = model.getAction();
                                var params = JSON.parse(model.getParams());
                                if(model.isNewStudent()){

                                }
                                return this.BackgroundNavigate(controller, action, params);
                            }
                        }, this);
            },

            [[chlk.models.discipline.SetDisciplineModel]],
            function setDisciplineAction(model){
//                var disciplines = model.getDisciplines();
//                var disc = disciplines.length > 0 ? disciplines[0] : null;
                var result = this.disciplineService
                    .setDiscipline(model)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        /*var controller = model.getController() || 'discipline';
                        var action = model.getAction() || 'list';
                        var params = JSON.parse(model.getParams()) || [];
                        return this.Redirect(controller, action, params);*/
                        model.setId(data.getId());
                        return model;//model.getDisciplines()[0];
                    }, this);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String, Boolean]],
            function showStudentDisciplineAction(studentId, date_, controller_, action_, params_, isNew_) {
                var result = this.listStudentDiscipline_(studentId, date_)
                    .then(function(model){
                        model.setTarget(chlk.controls.getActionLinkControlLastNode());
                        model.setAbleEdit(this.userIsAdmin() || this.userInRole(chlk.models.common.RoleEnum.TEACHER));
                        if(controller_)
                            model.setController(controller_);
                        if(action_)
                            model.setAction(action_);
                        if(params_)
                            model.setParams(params_);
                        if(isNew_)
                            model.setNewStudent(true);
                        return model;
                    }, this);
                return this.ShadeView(chlk.activities.discipline.DisciplineDayPopup, result);
            },

            [[chlk.models.attendance.AttendanceStudentBox]],
            VOID, function showStudentBoxAction(model) {
                var date = model.getDate() ? model.getDate().format('mm-dd-yy') : '';
                this.showStudentDisciplineAction(model.getId(), model.getDate(), 'discipline', 'list',
                    JSON.stringify([true, model.getGradeLevelsIds(), date]), true);
            },

            VOID, function addStudentClickAction(){

            }
        ])
});