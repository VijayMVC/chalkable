REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeople*/
    CLASS(
        'SchoolPeople', [
            ArrayOf(chlk.models.people.User), 'users',
            chlk.models.school.SchoolDetails, 'schoolInfo'
        ]);
});
