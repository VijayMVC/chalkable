REQUIRE('chlk.models.id.AlternateScoreId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.AlternateScore*/
    CLASS('AlternateScore', [
        chlk.models.id.AlternateScoreId, 'id',
        String, 'name',
        String, 'description',

        [ria.serialize.SerializeProperty('includeinaverage')],
        Boolean, 'includeInAverage',

        [ria.serialize.SerializeProperty('percentofmaximumscore')],
        Number, 'percentOfMaximumScore'
    ]);
});
