REQUIRE('chlk.models.standard.StandardGrading');
REQUIRE('chlk.models.standard.Standard');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.StandardGradings*/
    CLASS(
        'StandardGradings', [
            ArrayOf(chlk.models.standard.StandardGrading), 'items',

            [ria.serialize.SerializeProperty('numericavg')],
            Number, 'numericAvg',

            [ria.serialize.SerializeProperty('alphagradenameavg')],
            String, 'alphaGradeNameAvg',

            chlk.models.standard.Standard, 'standard'
        ]);
});
