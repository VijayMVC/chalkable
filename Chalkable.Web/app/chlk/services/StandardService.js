REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');

REQUIRE('chlk.models.academicBenchmark.Standard');
REQUIRE('chlk.models.id.CCStandardCategoryId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.StandardService*/
    CLASS(
        'StandardService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSubjects(classId_) {
                return this.get('Standard/GetStandardSubject.json', ArrayOf(chlk.models.standard.StandardSubject), {
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId]],
            ria.async.Future, function getStandards(classId_, subjectId_, standardId_) {
                return this.get('Standard/GetStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    classId: classId_ && classId_.valueOf(),
                    subjectId: subjectId_ && subjectId_.valueOf(),
                    parentStandardId: standardId_ && standardId_.valueOf(),
                    activeOnly: true
                });
            },

            [[Array]],
            ria.async.Future, function getStandardsList(ids) {
                return this.get('Standard/GetStandardsByIds.json', ArrayOf(chlk.models.standard.Standard), {
                    ids: this.arrayToIds(ids)
                }).then(function(model){
                    var res = [];
                    ids.forEach(function(id){
                        res.push(model.filter(function(item){return item.getStandardId() == id})[0]);
                    });
                    return res;
                });
            },

            [[chlk.models.id.ClassId, Boolean, String]],
            ria.async.Future, function searchStandardsByClassId(classId, activeOnly, filter) {
                return this.get('Standard/SearchStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    filter: filter,
                    classId: classId && classId.valueOf(),
                    activeOnly: activeOnly
                });
            },

            [[String, chlk.models.id.ClassId]],
            ria.async.Future, function searchStandards(filter, classId_) {
                return this.get('Standard/SearchStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    filter: filter,
                    activeOnly: true,
                    classId: classId_ && classId_.valueOf()
                }).then(function(items){
                    items.forEach(function(item){
                        item.setDeepest(true);
                    });
                    return items;
                });
            }
        ])
});