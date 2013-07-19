REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.class.ClassForTopBar');

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
            }
        ])
});