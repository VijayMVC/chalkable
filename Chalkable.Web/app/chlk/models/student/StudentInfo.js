REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.HealthCondition');
REQUIRE('chlk.models.student.StudentContact');
REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        ArrayOf(chlk.models.student.StudentContact), 'studentContacts',

        chlk.models.grading.GradeLevel, 'gradeLevel',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.studentContacts = SJX.fromArrayOfDeserializables(raw.studentcontacts, chlk.models.student.StudentContact);
            this.gradeLevel = SJX.fromDeserializable(raw.gradelevel, chlk.models.grading.GradeLevel);
        }
    ]);
});