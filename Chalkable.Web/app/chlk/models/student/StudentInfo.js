REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        [ria.serialize.SerializeProperty('gradelevel')],
        chlk.models.grading.GradeLevel, 'gradeLevel',
        ArrayOf(chlk.models.people.User), 'parents'
    ]);
});