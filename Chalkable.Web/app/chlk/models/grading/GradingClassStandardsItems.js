REQUIRE('chlk.models.grading.GradingClassStandardItem');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassStandardsItems*/
    CLASS(
        'GradingClassStandardsItems', [
            ArrayOf(chlk.models.grading.GradingClassStandardItem), 'items',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.common.NameId, 'gradingPeriod',

            Number, 'avg'
        ]);
});
