REQUIRE('chlk.models.class.Class');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassForTopBar*/
    CLASS(
        'ClassForTopBar', EXTENDS(chlk.models.class.Class), [
            String, 'controller',
            String, 'action',
            Array, 'params',
            Boolean, 'pressed',
            Number, 'index',
            Boolean, 'disabled'
        ]);
});
