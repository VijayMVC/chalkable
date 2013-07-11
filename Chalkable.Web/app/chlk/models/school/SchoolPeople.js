REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeople*/
    CLASS(
        'SchoolPeople', [
            ArrayOf(chlk.models.people.User), 'users',
            ArrayOf(chlk.models.common.NameId), 'roles',
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            Number, 'selectedIndex',
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo'
        ]);
});
