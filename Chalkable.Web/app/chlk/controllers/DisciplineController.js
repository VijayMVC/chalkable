REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DisciplineService');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineList');
REQUIRE('chlk.models.discipline.DisciplineInputModel');
REQUIRE('chlk.activities.discipline.DisciplineSummaryPage');
REQUIRE('chlk.activities.discipline.SetDisciplineDialog');


NAMESPACE('chlk.controllers', function(){
    ""
    /** @class chlk.controllers.DisciplineController */

    CLASS(
        'DisciplineController', EXTENDS(chlk.controllers.BaseController),[

            [ria.mvc.Inject],
            chlk.services.DisciplineService, 'disciplineService',

            [ria.mvc.Inject],
            chlk.services.DisciplineTypeService, 'disciplineTypeService',

            [[Number, chlk.models.common.ChlkDate]],
            function listAction(start_, date_){
                var res = this.disciplineService.list(date_, start_|0)
                              .attach(this.validateResponse_());
                return this.PushView(chlk.activities.discipline.DisciplineSummaryPage, res);
            },

            [[Number, chlk.models.common.ChlkDate]],
            function pageAction(start, date_){
                var res = this.disciplineService.list(date_ | null, start)
                              .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.discipline.DisciplineSummaryPage, res);
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
            }
        ])
});