REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.people.UsersList');

NAMESPACE('chlk.models.school', function () {
    "use strict";
    /** @class chlk.models.school.SchoolPeoplePart*/
    CLASS(
        'SchoolPeoplePart', EXTENDS(chlk.models.people.UsersList), [
            chlk.models.id.SchoolId, 'schoolId',
            Number, 'roleId',
            String, 'rolesId',
            Number, 'gradeLevelId'
        ]);
});
