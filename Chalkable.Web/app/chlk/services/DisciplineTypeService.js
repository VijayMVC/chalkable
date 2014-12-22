REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.id.DisciplineTypeId');

NAMESPACE('chlk.services', function(){

    "use strict";
    /** @class chlk.services.DisciplineTypeService*/

    CLASS(
        'DisciplineTypeService', EXTENDS(chlk.services.BaseService), [

            [[Number, Number]],
            ria.async.Future, function getPaginatedDisciplineTypes(start_, count_){
                return this.getPaginatedList('DisciplineType/List.json', chlk.models.discipline.DisciplineType, {
                   start: start_ | 0,
                   count: count_ | 10
                });
            },

            ria.async.Future, function getDisciplineTypes(){
                return this.getArray('DisciplineType/List.json', chlk.models.discipline.DisciplineType, {
                    count: Number.MAX_VALUE
                });
            },

            [[chlk.models.id.DisciplineTypeId]],
            ria.async.Future, function getDisciplineTypeInfo(id){
                return this.get('DisciplineType/Info.json', chlk.models.discipline.DisciplineType, {
                    disciplineTypeId: id.valueOf()
                });
            },

            [[String, Number]],
            ria.async.Future, function addDisciplineType(name, score){
                return this.post('DisciplineType/Create.json', chlk.models.discipline.DisciplineType,{
                    name: name,
                    score: score
                });
            },

            [[chlk.models.id.DisciplineTypeId, String, Number]],
            ria.async.Future, function updateDisciplineType(id, name, score){
                return this.post('DisciplineType/Update.json', chlk.models.discipline.DisciplineType,{
                    disciplineTypeId: id.valueOf(),
                    name: name,
                    score: score
                });
            },

            [[chlk.models.id.DisciplineTypeId, String, Number]],
            ria.async.Future, function saveDisciplineType(id_, name, score) {
                if (id_ && id_.valueOf())
                    return this.updateDisciplineType(id_, name, score);
                return this.addDisciplineType(name, score);
            },


            [[chlk.models.id.DisciplineTypeId]],
            ria.async.Future, function removeDisciplineType(id){
                return this.post('DisciplineType/Delete.json', chlk.models.discipline.DisciplineType, {
                    disciplineTypeId: id.valueOf()
                });
            }


    ]);
});