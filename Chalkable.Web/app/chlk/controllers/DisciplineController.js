REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineList');
REQUIRE('chlk.models.discipline.DisciplineInputModel');
REQUIRE('chlk.models.discipline.SetDisciplineListModel');
REQUIRE('chlk.models.discipline.PaginatedListByDateModel');

REQUIRE('chlk.activities.discipline.DisciplineSummaryPage');
REQUIRE('chlk.activities.discipline.SetDisciplineDialog');
REQUIRE('chlk.activities.discipline.DayDisciplinePopup');
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
            chlk.services.ClassService, 'classService',


            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.common.ChlkDate, Number, Number]],
            function listAction(date_, pageSize_, pageIndex_){
                var res = this.disciplineList_(pageIndex_, pageSize_, date_);
                return this.PushView(chlk.activities.discipline.DisciplineSummaryPage, res);
            },

            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.common.ChlkDate, Number, Number]],
            function pageAction(date_, pageSize, pageIndex){
                var res = this.disciplineList_(pageIndex, pageSize, date_);
                return this.UpdateView(chlk.activities.discipline.DisciplineSummaryPage, res);
            },

            [chlk.controllers.SidebarButton('discipline')],
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function classListAction(classId_, date_){
                if(!classId_ || !classId_.valueOf())
                    return this.Redirect('discipline', 'list', [date_]);
                var res = ria.async.wait([
                        this.disciplineService.getClassDisciplines(classId_, date_, 0),
                        this.disciplineTypeService.getDisciplineTypes()
                    ])
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var classes = this.classService.getClassesForTopBar(true);
                        var classBarData = new chlk.models.classes.ClassesForTopBar(classes);
                        return new chlk.models.discipline.ClassDisciplinesViewData(
                            classBarData, classId_, result[0], result[1], date_, true
                        );
                    }, this);

                var activityClass = chlk.activities.discipline.ClassDisciplinesPage;
                var currentActivity = this.getView().getCurrent();
                if(currentActivity && activityClass == currentActivity.getClass()){
                    return this.UpdateView(activityClass, res);
                }
                return this.PushView(activityClass, res);
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
                        date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                        var classes = this.classService.getClassesForTopBar(true);
                        var classBarData = new chlk.models.classes.ClassesForTopBar(classes);
                        return new ria.async.DeferredData(new chlk.models.discipline.PaginatedListByDateModel(classBarData, data, date_));
                    }, this);
            },

            [[chlk.models.discipline.DisciplineInputModel]],
            function listStudentDisciplineAction(model){
                var res = this.listStudentDiscipline_(model.getPersonId(), model.getDisciplineDate());
                return this.ShadeView(chlk.activities.discipline.SetDisciplineDialog, res);
            },
            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String]],
            function showStudentDayDisciplinesAction(studentId, date, controller_, action_, params_){
                var res = this.listStudentDiscipline_(studentId, date)
                    .then(function(data){
                        var target = chlk.controls.getActionLinkControlLastNode();
                        return new chlk.models.discipline.DisciplinePopupViewData(target, null, data, controller_, action_, params_);
                    });
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
                        date = date || new chlk.models.common.ChlkDate(getDate());
                        return new chlk.models.discipline.DisciplineList(result[1], result[0], date);
                    }, this);
            },

            [[chlk.models.discipline.SetDisciplineListModel]],
            function setDisciplinesAction(model){
                var result = this.disciplineService
                    .setDisciplines(model)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        /*var controller = model.getController() || 'discipline';
                        var action = model.getAction() || 'list';
                        var params = JSON.parse(model.getParams()) || [];
                        return this.Redirect(controller, action, params);*/
                        return model.getDisciplines()[0];
                    }, this);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            }
        ])
});