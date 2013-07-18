REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.class.ClassForTopBar');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            Array, function getClassesForTopBar() {
                 return new ria.serialize.JsonSerializer().deserialize(window.classesAdvancedData, ArrayOf(chlk.models.class.ClassForTopBar));
            }
        ])
});