REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.grading.ClassPersonGradingInfo');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    /**@class chlk.models.grading.ClassPersonGradesByGradingPeriod*/
    CLASS('ClassPersonGradesByGradingPeriod',  [

        [ria.serialize.SerializeProperty('gradingperiod')],
        chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

        [ria.serialize.SerializeProperty('studentgradings')],
        ArrayOf(chlk.models.grading.ClassPersonGradingInfo), 'studentGradings'
    ]);
});