REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeoplePart*/
    CLASS(
        'SchoolPeoplePart', [
            chlk.models.common.PaginatedList, 'users',
            Boolean, 'byLastName',
            Number, 'selectedIndex',

            chlk.models.id.SchoolId, 'schoolId',
            Number, 'roleId',
            Number, 'gradeLevelId',
            Boolean, 'byLastName'
        ]);
});
