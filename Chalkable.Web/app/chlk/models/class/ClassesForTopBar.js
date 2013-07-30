REQUIRE('chlk.models.class.ClassForTopBar');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.ClassesForTopBar*/
    CLASS(
        'ClassesForTopBar', [
            ArrayOf(chlk.models.class.ClassForTopBar), 'topItems',
            chlk.models.id.ClassId, 'selectedItemId',
            Boolean, 'disabled'
        ]);
});
