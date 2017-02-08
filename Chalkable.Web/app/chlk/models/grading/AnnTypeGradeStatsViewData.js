REQUIRE('chlk.models.grading.GradingStatsByDateViewData');
REQUIRE('chlk.models.id.AnnTypeGradeStatsId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.AnnTypeGradeStatsViewData*/
    CLASS(
        'AnnTypeGradeStatsViewData', [
            [ria.serialize.SerializeProperty('gradeperdate')],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'gradePerDate',

            chlk.models.id.AnnTypeGradeStatsId, 'id',

            [ria.serialize.SerializeProperty('typename')],
            String, 'typeName',

            Number, 'value',

            [ria.serialize.SerializeProperty('droplowest')],
            Boolean, 'dropLowest',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle'
        ]);
});
