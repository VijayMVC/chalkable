REQUIRE('chlk.models.id.GradingScaleId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradingScale*/
    CLASS('GradingScale', [
        chlk.models.id.GradingScaleId, 'id',
        String, 'name',
        String, 'description',
        [ria.serialize.SerializeProperty('homegradetodisplay')],
        Number, 'homeGradeToDisplay'
    ]);
});
