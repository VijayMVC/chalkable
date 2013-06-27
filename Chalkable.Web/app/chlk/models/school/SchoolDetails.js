REQUIRE('chlk.models.School');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolDetails*/
    CLASS(
        'SchoolDetails', EXTENDS(chlk.models.School), [
            Number, 'statusNumber',
            String, 'status',
            ArrayOf(Number), 'buttons',
            ArrayOf(String), 'emails'
        ]);
});
