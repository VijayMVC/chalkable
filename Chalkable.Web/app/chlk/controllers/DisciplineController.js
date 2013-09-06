REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineList');
REQUIRE('chlk.models.discipline.DisciplineInputModel');
REQUIRE('chlk.models.discipline.SetDisciplineListModel');
REQUIRE('chlk.models.discipline.PaginatedListByDateModel');
REQUIRE('chlk.activities.discipline.DisciplineSummaryPage');
REQUIRE('chlk.activities.discipline.SetDisciplineDialog');


NAMESPACE('chlk.controllers', function(){
    "use strict";
    /** @class chlk.controllers.DisciplineController */

    CLASS(
        'DisciplineController', EXTENDS(chlk.controllers.BaseController),[

            [ria.mvc.Inject],
            chlk.services.DisciplineService, 'disciplineService',

            [ria.mvc.Inject],
            chlk.services.DisciplineTypeService, 'disciplineTypeService',

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

            [[Number, Number, chlk.models.common.ChlkDate]],
            ria.async.Future, function disciplineList_(pageIndex_, pageSize_, date_){
                var start = 0;
                if(pageIndex_ && pageSize_)
                    start = pageIndex_ * pageSize_;
                return this.disciplineService.list(date_, start)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                        return new ria.async.DeferredData(new chlk.models.discipline.PaginatedListByDateModel(data, date_));
                    });
            },

            [[chlk.models.discipline.DisciplineInputModel]],
            function listStudentDisciplineAction(model){
                //todo move disciplineTypes to session
                var date = model.getDisciplineDate();
                var res = ria.async.wait([
                    this.disciplineTypeService.getDisciplineTypes(),
                    this.disciplineService.listStudentDiscipline(null, model.getPersonId(), date)
                ]).attach(this.validateResponse_())
                  .then(function(result){
                        date = date || new chlk.models.common.ChlkDate(getDate());
                        return new chlk.models.discipline.DisciplineList(result[1], result[0], date);
                  }, this);
                return this.ShadeView(chlk.activities.discipline.SetDisciplineDialog, res); //todo create activities
            },

            [[chlk.models.discipline.SetDisciplineListModel]],
            function setDisciplinesAction(model){
               var res = this.disciplineService.setDisciplines(model).attach(this.validateResponse_());
               this.Redirect('discipline', 'list', []);
            }
        ])
});