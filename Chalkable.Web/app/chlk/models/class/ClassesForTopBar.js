REQUIRE('chlk.models.class.Class');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.class', function () {
    "use strict";

    /** @class chlk.models.class.ClassId*/
    IDENTIFIER('ClassId');

    /** @class chlk.models.class.ClassesForTopBar*/
    CLASS(
        'ClassesForTopBar', [
            ArrayOf(chlk.models.class.Class), 'topItems',
            chlk.models.id.ClassId, 'selectedItemId',
            Boolean, 'disabled'
        ]);
});
