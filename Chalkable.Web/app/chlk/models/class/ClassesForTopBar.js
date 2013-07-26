REQUIRE('chlk.models.class.ClassForTopBar');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.ClassesForTopBar*/
    CLASS(
        'ClassesForTopBar', [
            ArrayOf(chlk.models.class.ClassForTopBar), 'topItems',
            chlk.models.class.ClassId, 'selectedItemId'
        ]);
});
