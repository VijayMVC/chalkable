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
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo',

            [[chlk.models.people.UsersList, ArrayOf(chlk.models.common.NameId),
                ArrayOf(chlk.models.common.NameId), chlk.models.school.SchoolPeopleSummary]],
            function $(usersList, roles, gradeLevels, schoolInfo){
                BASE();
                this.setUsersPart(usersList);
                this.setRoles(roles);
                this.setGradeLevels(gradeLevels);
                this.setSchoolInfo(schoolInfo);
            }
        ]);
});
