REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassForTopBar*/
    CLASS(
        'ClassForTopBar', EXTENDS(chlk.models.classes.Class), [
            String, 'controller',
            String, 'action',
            Array, 'params',
            Boolean, 'pressed',
            Number, 'index',
            Boolean, 'disabled'
        ]);
});
