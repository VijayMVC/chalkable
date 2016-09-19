REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.ProgramId');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.recipients.Program');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.recipients.BaseViewData');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.UsersListViewData*/
    CLASS(
        'UsersListViewData', EXTENDS(chlk.models.recipients.BaseViewData), [
            Boolean, 'byLastName',
            chlk.models.id.SchoolId, 'schoolId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.GradeLevelId, 'gradeLevelId',
            chlk.models.id.ProgramId, 'programId',
            String, 'filter',
            Number, 'start',
            Number, 'count',
            String, 'submitType',
            Boolean, 'my',
            Boolean, 'hasAccessToLE',
            chlk.models.common.PaginatedList, 'users',
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',
            ArrayOf(chlk.models.school.School), 'schools',
            ArrayOf(chlk.models.classes.Class), 'classes',
            ArrayOf(chlk.models.recipients.Program), 'programs',

            [[chlk.models.recipients.SelectorModeEnum, Boolean, Boolean, Boolean, chlk.models.common.PaginatedList,
                ArrayOf(chlk.models.grading.GradeLevel), ArrayOf(chlk.models.school.School), ArrayOf(chlk.models.recipients.Program)]],
            function $(selectorMode_, hasAccessToAllStudents_, hasOwnStudents_, my_, users_, gradeLevels_, schools_, programs_){
                BASE(selectorMode_, hasAccessToAllStudents_, hasOwnStudents_);
                my_ && this.setMy(my_);
                users_ && this.setUsers(users_);
                gradeLevels_ && this.setGradeLevels(gradeLevels_);
                schools_ && this.setSchools(schools_);
                programs_ && this.setPrograms(programs_);
            }
        ]);
});
