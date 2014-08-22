REQUIRE('chlk.models.id.AlphaGradeId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.AlphaGrade*/
    CLASS('AlphaGrade', [
        chlk.models.id.AlphaGradeId, 'id',
        String, 'name',
        String, 'description'
    ]);
});
