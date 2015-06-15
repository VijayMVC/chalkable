REQUIRE('chlk.models.id.StandardsGradingScaleId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradingScale*/
    CLASS('GradingScale', [
        chlk.models.id.StandardsGradingScaleId, 'id',
        String, 'name',
        String, 'description',
        [ria.serialize.SerializeProperty('homegradetodisplay')],
        Number, 'homeGradeToDisplay'
    ]);
});
