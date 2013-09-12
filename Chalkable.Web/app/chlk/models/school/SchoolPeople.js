REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.people.UsersList');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeople*/
    CLASS(
        'SchoolPeople', [
            chlk.models.people.UsersList, 'usersPart', //todo: rename
            ArrayOf(chlk.models.common.NameId), 'roles',
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo'
        ]);
});
