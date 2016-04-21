REQUIRE('chlk.models.people.UsersList');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.admin', function (){
    "use strict";

    /**@class chlk.models.admin.PersonsForAdmin*/

    CLASS('PersonsForAdmin', [
        chlk.models.people.UsersList, 'usersList',
        ArrayOf(chlk.models.common.Role), 'roles',
        ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',

        [[chlk.models.people.UsersList, ArrayOf(chlk.models.common.Role), ArrayOf(chlk.models.grading.GradeLevel)]],
        function $(usersList, roles, gradeLevels){
//            VALIDATE_ARG('gradeLevels', [ArrayOf(chlk.models.common.NameId), ArrayOf(chlk.models.grading.GradeLevel)], gradeLevels);
            BASE();

//            var gls = gradeLevels.map(function (_) {
//                if (_ instanceof chlk.models.grading.GradeLevel)
//                    return new chlk.models.common.NameId(gradeLevels[i].getId().valueOf(), gradeLevels[i].getName());
//
//                return _;
//            });
            this.setUsersList(usersList);
            this.setRoles(roles);
            this.setGradeLevels(gradeLevels);
        }
    ]);
});