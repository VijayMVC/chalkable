REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.classes.ClassForTopBar');
REQUIRE('chlk.models.classes.ClassForWeekMask');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassSummary');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            ArrayOf(chlk.models.classes.ClassForTopBar), 'classesToFilter',
            ArrayOf(chlk.models.classes.ClassForTopBar), 'classesToFilterWithAll',

            [[Boolean]],
            Array, function getClassesForTopBar(withAll_) {
                var res = this.getClassesToFilter(), res1 = this.getClassesToFilterWithAll();
                if(res)
                    return withAll_ ? res1 : res;
                res = new ria.serialize.JsonSerializer().deserialize(window.classesToFilter, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilter(res);
                var classesToFilterWithAll = window.classesToFilter.slice();
                classesToFilterWithAll.unshift({
                    name: 'All',
                    description: 'All',
                    id: ''
                });
                res1 = new ria.serialize.JsonSerializer().deserialize(classesToFilterWithAll, ArrayOf(chlk.models.classes.ClassForTopBar));
                this.setClassesToFilterWithAll(res1);
                return withAll_ ? res1 : res;
            },

            //TODO: refactor
            [[chlk.models.id.ClassId]],
            chlk.models.classes.ClassForWeekMask, function getClassAnnouncementInfo(id){
                var res = window.classesInfo[id.valueOf()];
                res.classId = id.valueOf();
                res = new ria.serialize.JsonSerializer().deserialize(res, chlk.models.classes.ClassForWeekMask);
                return res;
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSummary(classId) {
                return this.get('Class/ClassSummary.json', chlk.models.classes.ClassSummary, {
                    classId: classId.valueOf()
                });
            }
        ])
});