REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.ProgramId');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.recipients.Program');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.UsersListViewData*/
    CLASS(
        'UsersListViewData', [
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
            chlk.models.common.PaginatedList, 'users',
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',
            ArrayOf(chlk.models.school.School), 'schools',
            ArrayOf(chlk.models.classes.Class), 'classes',
            ArrayOf(chlk.models.recipients.Program), 'programs',

            [[chlk.models.common.PaginatedList, Boolean,
                ArrayOf(chlk.models.grading.GradeLevel), ArrayOf(chlk.models.school.School), ArrayOf(chlk.models.recipients.Program),
                ArrayOf(chlk.models.classes.Class), chlk.models.id.GradeLevelId, chlk.models.id.SchoolId, chlk.models.id.ProgramId,
                chlk.models.id.ClassId, Boolean, String
            ]],
            function $(users_, my_, gradeLevels_, schools_, programs_, classes_, gradeLevelId_, schoolId_, programId_, classId_, byLastName_, filter_){
                BASE();
                users_ && this.setUsers(users_);
                my_ && this.setMy(my_);
                gradeLevels_ && this.setGradeLevels(gradeLevels_);
                schools_ && this.setSchools(schools_);
                programs_ && this.setPrograms(programs_);
                classes_ && this.setClasses(classes_);
                gradeLevelId_ && this.setGradeLevelId(gradeLevelId_);
                schoolId_ && this.setSchoolId(schoolId_);
                programId_ && this.setProgramId(programId_);
                classId_ && this.setClassId(classId_);
                byLastName_ && this.setByLastName(byLastName_);
                filter_ && this.setFilter(filter_);
            }
        ]);
});
