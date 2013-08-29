REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.class.ClassForWeekMask');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.class.ClassSummary');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            ArrayOf(chlk.models.class.ClassForTopBar), 'classesToFilter',
            ArrayOf(chlk.models.class.ClassForTopBar), 'classesToFilterWithAll',

            [[Boolean]],
            Array, function getClassesForTopBar(withAll_) {
                var res = this.getClassesToFilter(), res1 = this.getClassesToFilterWithAll();
                if(res)
                    return withAll_ ? res1 : res;
                res = new ria.serialize.JsonSerializer().deserialize(window.classesToFilter, ArrayOf(chlk.models.class.ClassForTopBar));
                this.setClassesToFilter(res);
                var classesToFilterWithAll = window.classesToFilter.slice();
                classesToFilterWithAll.unshift({
                    name: 'All',
                    description: 'All',
                    id: ''
                });
                res1 = new ria.serialize.JsonSerializer().deserialize(classesToFilterWithAll, ArrayOf(chlk.models.class.ClassForTopBar));
                this.setClassesToFilterWithAll(res1);
                return withAll_ ? res1 : res;
            },

            [[chlk.models.id.ClassId]],
            chlk.models.class.ClassForWeekMask, function getClassAnnouncementInfo(id){
                var res = window.classesInfo[id.valueOf()];
                res = new ria.serialize.JsonSerializer().deserialize(res, chlk.models.class.ClassForWeekMask);
                return res;
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSummary(classId) {
                return this.get('Class/ClassSummary.json', chlk.models.class.ClassSummary, {
                    classId: classId.valueOf()
                });
            }
        ])
});