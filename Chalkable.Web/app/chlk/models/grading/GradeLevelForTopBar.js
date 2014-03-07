REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradeLevelForTopBar*/
    CLASS(
        'GradeLevelForTopBar', EXTENDS(chlk.models.grading.GradeLevel), [
            String, 'controller',
            String, 'action',
            Array, 'params',
            Number, 'index',
            Boolean, 'pressed'
        ]);
});
