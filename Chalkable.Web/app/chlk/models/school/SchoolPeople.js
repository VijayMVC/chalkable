REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeople*/
    CLASS(
        'SchoolPeople', [
            chlk.models.common.PaginatedList, 'users',
            ArrayOf(chlk.models.common.NameId), 'roles',
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            Boolean, 'byLastName',
            Number, 'selectedIndex',
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo'
        ]);
});
