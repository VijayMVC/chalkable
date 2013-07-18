REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.school.SchoolPeoplePart');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeople*/
    CLASS(
        'SchoolPeople', [
            chlk.models.school.SchoolPeoplePart, 'usersPart',
            ArrayOf(chlk.models.common.NameId), 'roles',
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo'
        ]);
});
