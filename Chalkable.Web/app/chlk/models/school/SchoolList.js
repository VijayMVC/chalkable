REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    /** @class chlk.models.SchoolList*/
    CLASS(
        'SchoolList', [
            ArrayOf(chlk.models.school.School), 'items'
        ]);
});