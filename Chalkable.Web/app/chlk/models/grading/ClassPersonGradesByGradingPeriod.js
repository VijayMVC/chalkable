REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.grading.ClassPersonGradingInfo');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.grading.ClassPersonGradesByGradingPeriod*/
    CLASS(UNSAFE, 'ClassPersonGradesByGradingPeriod', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

        ArrayOf(chlk.models.grading.ClassPersonGradingInfo), 'studentGradings',

        VOID, function deserialize(raw) {
            this.gradingPeriod = SJX.fromDeserializable(raw.gradingperiod, chlk.models.schoolYear.GradingPeriod);
            this.studentGradings = SJX.fromArrayOfDeserializables(raw.classavgs, chlk.models.grading.ClassPersonGradingInfo);
        }
    ]);
});