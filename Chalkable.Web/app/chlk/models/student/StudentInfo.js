REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.people.HealthCondition');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentInfo*/

    CLASS('StudentInfo', EXTENDS(chlk.models.people.User),[

        chlk.models.grading.GradeLevel, 'gradeLevel',
        ArrayOf(chlk.models.people.User), 'parents',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.gradeLevel = SJX.fromDeserializable(raw.gradelevel, chlk.models.grading.GradeLevel);
            this.parents = SJX.fromArrayOfDeserializables(raw.parents, chlk.models.people.User);
        }
    ]);
});