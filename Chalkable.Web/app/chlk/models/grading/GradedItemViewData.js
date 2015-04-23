REQUIRE('chlk.models.id.GradedItemId');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.GradedItemViewData*/
    CLASS('GradedItemViewData', [
        chlk.models.id.GradedItemId, 'id',
        String, 'name',
        [ria.serialize.SerializeProperty('appearsonreportcard')],
        Boolean, 'appearsOnReportCard'
    ]);
});
