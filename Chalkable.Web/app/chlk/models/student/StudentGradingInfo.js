REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.grading.ClassPersonGradesByGradingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentGradingInfo*/

    CLASS('StudentGradingInfo', EXTENDS(chlk.models.people.ShortUserInfo),[

        [ria.serialize.SerializeProperty('gradesbygradingperiod')],
        ArrayOf(chlk.models.grading.ClassPersonGradesByGradingPeriod), 'gradesByGradingPeriod'

    ]);
});