REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.class.ClassForWeekMask');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            ArrayOf(chlk.models.class.ClassForTopBar), 'classesToFilter',

            Array, function getClassesForTopBar() {
                var res = this.getClassesToFilter();
                if(res)
                    return res;
                res = new ria.serialize.JsonSerializer().deserialize(window.classesToFilter, ArrayOf(chlk.models.class.ClassForTopBar));
                this.setClassesToFilter(res);
                return res;
            },

            [[chlk.models.id.ClassId]],
            chlk.models.class.ClassForWeekMask, function getClassAnnouncementInfo(id){
                var res = window.classesInfo[id.valueOf()];
                res = new ria.serialize.JsonSerializer().deserialize(res, chlk.models.class.ClassForWeekMask);
                return res;
            }
        ])
});