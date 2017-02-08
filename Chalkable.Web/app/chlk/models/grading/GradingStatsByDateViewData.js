REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingStatsByDateViewData*/
    CLASS(
        'GradingStatsByDateViewData', [
            chlk.models.common.ChlkDate, 'date',
            Number, 'avg'
        ]);
});
