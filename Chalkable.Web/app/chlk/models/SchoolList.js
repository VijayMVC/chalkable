REQUIRE('chlk.models.School');

NAMESPACE('chlk.models', function () {
    "use strict";

    /** @class chlk.models.SchoolList*/
    CLASS(
        'SchoolList', [
            ArrayOf(chlk.models.School), 'items'
        ]);
});