REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DisciplineTypeService');
REQUIRE('chlk.activities.discipline.DisciplineTypePage');
REQUIRE('chlk.activities.discipline.AddDisciplineTypeDialog');

REQUIRE('chlk.models.id.DisciplineTypeId');
REQUIRE('chlk.models.discipline.DisciplineType');

NAMESPACE('chlk.controllers', function(){
    "use strict";

    /**@class chlk.controllers.DisciplineTypeController */

    CLASS('DisciplineTypeController', EXTENDS(chlk.controllers.BaseController),[

        [ria.mvc.Inject],
        chlk.services.DisciplineTypeService, 'disciplineTypeService',

        [[Number, Number, Boolean]],
        function listAction(isPageAction_, pageSize_, pageIndex_){
            var start = pageIndex_ && pageSize_ ? (pageIndex_ / pageSize_) : null
            var res = this.disciplineTypeService.getPaginatedDisciplineTypes(start, pageSize_)
                                                .attach(this.validateResponse_());
            return isPageAction_ ? this.UpdateView(chlk.activities.discipline.DisciplineTypePage, res)
                                 : this.PushView(chlk.activities.discipline.DisciplineTypePage, res);
        },

        function addAction(){
            var res = new ria.async.DeferredData(new chlk.models.discipline.DisciplineType);
            return this.ShadeView(chlk.activities.discipline.AddDisciplineTypeDialog, res);
        },

        [[chlk.models.id.DisciplineTypeId]],
        function updateAction(disciplineTypeId){
            var res = this.disciplineTypeService.getDisciplineTypeInfo(disciplineTypeId)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.discipline.AddDisciplineTypeDialog, res);
        },
        [[chlk.models.discipline.DisciplineType]],
        function saveAction(model){
            var res = this.disciplineTypeService.saveDisciplineType(model.getId(), model.getName(), model.getScore())
                .attach(this.validateResponse_())
                .then(function(data){
                    this.view.getCurrent().close();
                    return this.disciplineTypeService.getPaginatedDisciplineTypes(0, null);
                }, this);
            return this.UpdateView(chlk.activities.discipline.DisciplineTypePage, res);
        },

        [[chlk.models.id.DisciplineTypeId]],
        function deleteAction(id){
            var res = this.disciplineTypeService.removeDisciplineType(id)
                .attach(this.validateResponse_())
                .then(function (data){
                    return this.disciplineTypeService.getPaginatedDisciplineTypes(0, null);
                }, this);
            return this.UpdateView(chlk.activities.discipline.DisciplineTypePage, res);
        }

    ]);
});