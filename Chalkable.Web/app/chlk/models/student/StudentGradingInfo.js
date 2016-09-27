REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.grading.ClassPersonGradesByGradingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";


    var SJX = ria.serialize.SJX;
    /**@class chlk.models.student.StudentGradingInfo*/



    CLASS(UNSAFE, 'StudentGradingInfo', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable),[

        ArrayOf(chlk.models.grading.ClassPersonGradesByGradingPeriod), 'gradesByGradingPeriod',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw.student);
            this.gradesByGradingPeriod = SJX.fromArrayOfDeserializables(raw.gradesbygradingperiod, chlk.models.grading.ClassPersonGradesByGradingPeriod);
        }

    ]);
});